import { Injectable, OnDestroy, OnInit } from '@angular/core';
import { CacheBase } from '@shared/base/cache-base';
import { map, Observable, of, Subject, takeUntil, tap } from 'rxjs';

export interface ILookupItem {
  id: string;
}

@Injectable()
export abstract class LookupBase<T extends ILookupItem>
  extends CacheBase<T>
  implements OnInit, OnDestroy
{
  protected abstract lookupFunction: (ids: string[]) => Observable<T[]>;
  protected abstract itemUpdatedEvent$: Observable<string>;

  private readonly onDestroy = new Subject<void>();
  private onDestroy$ = this.onDestroy.asObservable();

  public ngOnInit(): void {
    this.subscribeToItemUpdatedEvent();
  }

  private subscribeToItemUpdatedEvent(): void {
    this.itemUpdatedEvent$.pipe(takeUntil(this.onDestroy$)).subscribe((id) => {
      this.removeFromCache(id);
    });
  }

  public getById(id: string, skipCache: boolean = false): Observable<T | undefined> {
    const itemInCache = !skipCache
      ? this.cache.value.find(
          (item) => item?.id != null && id != null && item.id.toLowerCase() === id.toLowerCase()
        )
      : null;
    if (itemInCache) {
      return of(itemInCache);
    } else {
      return this.lookupFunction([id]).pipe(
        map((items) => items?.[0]),
        tap((item) => {
          if (item && item.id) {
            this.addToOrReplaceInCache(item);
          }
        })
      );
    }
  }

  public getByIds(ids: string[], skipCache: boolean = false): Observable<T[]> {
    const itemsInCache = !skipCache
      ? this.cache.value.filter((item) =>
          ids.some(
            (id) => item?.id != null && id != null && item.id.toLowerCase() === id.toLowerCase()
          )
        )
      : [];
    if (itemsInCache.length === ids.length) {
      return of(itemsInCache);
    } else {
      return this.lookupFunction(ids).pipe(
        tap((items) => {
          if (items) {
            for (const item of items) {
              if (item && item.id) {
                this.addToOrReplaceInCache(item);
              }
            }
          }
        })
      );
    }
  }

  public ngOnDestroy(): void {
    this.onDestroy.next();
    this.onDestroy.complete();
  }
}