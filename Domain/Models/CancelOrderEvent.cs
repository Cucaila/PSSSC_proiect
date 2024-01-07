using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    [AsChoice]
    public static partial class CancelOrderEvent
    {
        public interface ICancelOrderEvent { }

        public record CancelOrderSucceededEvent : ICancelOrderEvent
        {
            public int NumberOfOrder { get; }
            public DateTime CancellationDate { get; }

            internal CancelOrderSucceededEvent(int numberOfOrder, DateTime cancellationDate)
            {
                NumberOfOrder = numberOfOrder;
                CancellationDate = cancellationDate;
            }
        }

        public record CancelOrderFailedEvent : ICancelOrderEvent
        {
            public string Reason { get; }

            internal CancelOrderFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}

