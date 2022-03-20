using System;

namespace Demo.Events.User
{
    public class UserCreatedEvent : Event<UserCreatedEvent, UserCreatedEventData>
    {
        public static UserCreatedEvent Create(string correlationId, Guid id, Guid createdBy)
        {
            var data = new UserCreatedEventData
            {
                CorrelationId = correlationId,
                Id = id,
                CreatedBy = createdBy
            };
            return new UserCreatedEvent
            {
                Topic = Topics.User,
                Subject = $"User/{data.Id}",
                Data = data,
                DataVersion = data.EventDataVersion,
                CorrelationId = correlationId
            };
        }
    }

    public class UserCreatedEventData : IEventData
    {
        public string EventDataVersion => "1.0";
        public string CorrelationId { get; set; }

        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
    }
}