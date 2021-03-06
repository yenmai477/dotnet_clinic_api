﻿using AutoMapper;
using Clinic.Core;
using Clinic.Dtos;
using Clinic.Models;
using Clinic.Models.Entities;
using Clinic.Persistents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Controllers
{
    [Authorize(Roles = Role.Admin + "," + Role.Employee)]
    [ApiController]
    [Route("/api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;


       
        public InvoiceController(IInvoiceRepository InvoiceRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.repository = InvoiceRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QueryResultDto<InvoiceDto>>> GetInvoices([FromQuery]  InvoiceQuery InvoiceQuery)
        {
            var filter = await repository.GetInvoices(InvoiceQuery);

            return Ok(mapper.Map<QueryResult<Invoice>, QueryResultDto<InvoiceDto>>(filter));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var Invoice = await repository.GetInvoice(id);

            if (Invoice == null)
                return NotFound();

            var InvoiceResult = mapper.Map<Invoice, InvoiceDetailDto>(Invoice);

            return Ok(InvoiceResult);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoice = await repository.GetInvoice(id);

            if (invoice == null)
                return NotFound();

            //mapper.Map<InvoiceDto, Invoice>(invoiceDto, invoice);

            if (invoiceDto.Status == "Đã thanh toán")
                invoice.Status = invoiceDto.Status;

            await unitOfWork.CompleteAsync();

            invoice = await repository.GetInvoice(invoice.InvoiceId);
            var result = mapper.Map<Invoice, InvoiceDto>(invoice);

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var Invoice = await repository.GetInvoice(id, includeRelated: false);

            if (Invoice == null)
                return NotFound();

            repository.Remove(Invoice);
            await unitOfWork.CompleteAsync();

            return new NoContentResult();
        }

    }
}
