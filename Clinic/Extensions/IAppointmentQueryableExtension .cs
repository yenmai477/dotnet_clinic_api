﻿using Clinic.Models;
using Clinic.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Clinic.Extensions
{
    public static class IAppointmentQueryableExtension
    {

        public static IQueryable<Appointment> ApplyFiltering(this IQueryable<Appointment> query, AppointmentQuery queryObj)
        {
            query = query.Include(Appointment => Appointment.Patient);

            if (!String.IsNullOrEmpty(queryObj.PatientName))
            {
                query = query.Where(appointment => appointment.Patient.FullName.Equals(queryObj.PatientName));
            }

            if(!String.IsNullOrEmpty(queryObj.Status))
            {
                query = query.Where(appointment => appointment.Status.Equals(queryObj.Status));
            }
               

            return query;
        }

    }
}

