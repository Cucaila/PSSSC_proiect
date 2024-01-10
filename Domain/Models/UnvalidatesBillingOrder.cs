﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record UnvalidatedBillingOrder(string OrderRegistrationCode, string OrderDescription, float OrderAmount, string OrderAddress, float OrderPrice)
    {

    }
}