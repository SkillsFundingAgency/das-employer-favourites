﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Api.Models
{
    public class ProviderData
    {
        public int Ukprn { get; set; }

        public List<int> LocationIds { get; set; }
    }
}
