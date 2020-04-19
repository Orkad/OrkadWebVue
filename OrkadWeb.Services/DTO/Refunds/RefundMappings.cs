﻿using OrkadWeb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrkadWeb.Services.DTO.Refunds
{
    public static class RefundMappings
    {
        public static RefundItem ToItem(this Refund refund) => new RefundItem
        {
            Id = refund.Id,
            Amount = refund.Amount,
            EmitterId = refund.Emitter.Id,
            ReceiverId = refund.Receiver.Id
        };
    }
}