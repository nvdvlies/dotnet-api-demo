import { Injectable } from '@angular/core';
import {
  ApiCurrentUserClient,
  ApiException,
  ProblemDetails,
  UserDto
} from '@api/api.generated.clients';
import { AuthService } from '@auth0/auth0-angular';
import { BehaviorSubject, catchError, Observable, of, switchMap, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  private user: UserDto | undefined;

  private isInitialized = new BehaviorSubject<boolean>(false);
  private initializationProblemDetails = new BehaviorSubject<
    ProblemDetails | ApiException | undefined
  >(undefined);

  public isInitialized$ = this.isInitialized.asObservable();
  public initializationProblemDetails$ = this.initializationProblemDetails.asObservable();

  constructor(
    private readonly apiCurrentUserClient: ApiCurrentUserClient,
    private readonly authService: AuthService
  ) {
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      if (!isAuthenticated) {
        this.user = undefined;
        this.isInitialized.next(false);
      }
    });
  }

  public init(skipCache: boolean = false): Observable<boolean> {
    if (skipCache || this.user == undefined) {
      return this.apiCurrentUserClient.getCurrentUserDetails().pipe(
        catchError((error: ProblemDetails | ApiException) => {
          this.initializationProblemDetails.next(error);
          return throwError(() => new Error('An error occured while initializing current user'));
        }),
        tap((response) => (this.user = response.user)),
        tap(() => this.isInitialized.next(true)),
        switchMap(() => of(true))
      );
    } else {
      return of(true);
    }
  }

  public get id(): string | undefined {
    return this.user?.id;
  }

  public get externalId(): string | undefined {
    return this.user?.externalId;
  }

  public get fullname(): string | undefined {
    return this.user?.fullname;
  }

  public get email(): string | undefined {
    return this.user?.email;
  }
}