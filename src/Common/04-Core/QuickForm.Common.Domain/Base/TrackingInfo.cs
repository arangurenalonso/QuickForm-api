﻿namespace QuickForm.Common.Domain;
public class TrackingInfo
{
    public string ClassOrigin { get; }
    public Guid TransactionId { get; }

    public TrackingInfo(string classOrigin, Guid transactionId)
    {
        ClassOrigin = classOrigin;
        TransactionId = transactionId;
    }
}
