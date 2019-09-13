// <copyright file="Organisation.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.Datalayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Monosoft.Service.OrganisationDB.Datalayer;

    /// <summary>
    /// Organisation description.
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ParentOrganisation
        /// </summary>
        public virtual Organisation ParentOrganisation { get; set; }

        /// <summary>
        /// Gets or sets the Cluster
        /// </summary>
        public virtual Cluster Cluster { get; set; }

        /// <summary>
        /// Gets or sets the Contracts
        /// </summary>
        public virtual ICollection<Contract> Contracts { get; set; }

        /// <summary>
        /// Gets or sets the Invoices
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; }

        /// <summary>
        /// Gets or sets the SubOrganisations
        /// </summary>
        public virtual ICollection<Organisation> SubOrganisations { get; set; }

        /// <summary>
        /// Gets or sets the Metadata
        /// </summary>
        public virtual ICollection<OrganisationMetadata> Metadata { get; set; }

        /// <summary>
        /// Create default organisation (Monosoft)
        /// </summary>
        /// <param name="organisation">DTO of organisation</param>
        internal static void CreateDefault(Auth.DTO.Organisation organisation)
        {
            var dborg = DataContext.Instance.Organisations.Where(x => x.Id == organisation.Id).FirstOrDefault();

            if (dborg == null)
            {
                var dbcluster = DataContext.Instance.Clusters.Where(x => x.ClusterId == organisation.Cluster.ClusterId).FirstOrDefault();

                dborg = new Organisation()
                {
                    Id = organisation.Id,
                    Name = organisation.Name,
                    Cluster = dbcluster
                };
                DataContext.Instance.Organisations.Add(dborg);
                DataContext.Instance.SaveChanges();
            }
        }

        /// <summary>
        /// Create a new organisation
        /// </summary>
        /// <param name="organisation">DTO containing the organisation definition</param>
        /// <param name="scope">In which scope is the organisation created</param>
        /// <returns>The created organisation</returns>
        public static Organisation Create(Monosoft.Auth.DTO.Organisation organisation, string scope)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            Organisation dborg = new Organisation();
            dborg.Name = organisation.Name;
            dborg.ParentOrganisation = DataContext.Instance.Organisations.Where(p => p.Id == organisation.ParentOrganisation).FirstOrDefault();
            dborg.Cluster = DataContext.Instance.Clusters.Where(p => p.ClusterId == organisation.Cluster.ClusterId).FirstOrDefault();

            if (dborg.Cluster != null) // dborg.ParentOrganisation != null &&  <-- fjernet pga. problem med første org. (i.e. vil altid være null)
            {
                DataContext.Instance.Organisations.Add(dborg);
                if (organisation.Contracts != null)
                {
                    foreach (var contract in organisation.Contracts)
                    {
                        Contract dbcontract = new Contract();
                        dbcontract.Organisation = dborg;
                        dbcontract.Base64File = contract.Base64File;
                        dbcontract.Description = contract.Description;
                        dbcontract.Interval = contract.Interval;
                        dbcontract.IsActive = contract.IsActive;
                        dbcontract.NextInvoicedate = contract.NextInvoicedate;
                        dbcontract.OngoingPayment = contract.OngoingPayment;
                        dbcontract.Payout = contract.Payout;
                        dbcontract.Title = contract.Title;
                        DataContext.Instance.Contracts.Add(dbcontract);
                    }
                }

                if (organisation.Invoices != null)
                {
                    foreach (var invoice in organisation.Invoices)
                    {
                        Invoice dbinvoice = new Invoice();
                        dbinvoice.Organisation = dborg;
                        dbinvoice.Amout = invoice.Amout;
                        dbinvoice.Base64File = invoice.Base64File;
                        dbinvoice.Details = invoice.Details;
                        dbinvoice.DueDate = invoice.DueDate;
                        dbinvoice.Invoicedate = invoice.Invoicedate;
                        dbinvoice.Status = invoice.Status;
                        dbinvoice.Titel = invoice.Titel;
                        DataContext.Instance.Invoices.Add(dbinvoice);
                    }
                }

                if (organisation.Metadata != null)
                {
                    foreach (var metadata in organisation.Metadata)
                    {
                        OrganisationMetadata meta = new OrganisationMetadata();
                        meta.Organisation = dborg;
                        meta.Key = metadata.Key;
                        // if (metadata.Scope != scope)
                        //{
                        //    throw new Exception("CRITICAL ERROR: Metadata scope does not match login scope");
                        //}

                        meta.Scope = metadata.Scope;
                        meta.Value = metadata.Value;

                        DataContext.Instance.OrganisationMetadatas.Add(meta);
                    }
                }

                DataContext.Instance.SaveChanges();
                return dborg;
            }

            return null;
        }

        /// <summary>
        /// Update an existing organisation
        /// </summary>
        /// <param name="organisation">DTO containing the organisation definition</param>
        /// <param name="scope">In which scope is the organisation updated</param>
        /// <returns>The updated organisation</returns>
        public static Organisation Update(Monosoft.Auth.DTO.Organisation organisation, string scope)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            Organisation dborg = DataContext.Instance.Organisations.Where(p => p.Id == organisation.Id).FirstOrDefault();
            if (dborg != null)
            {
                dborg.Name = organisation.Name;
                dborg.ParentOrganisation = DataContext.Instance.Organisations.Where(p => p.Id == organisation.ParentOrganisation).FirstOrDefault();
                dborg.Cluster = DataContext.Instance.Clusters.Where(p => p.ClusterId == organisation.Cluster.ClusterId).FirstOrDefault();

                if (dborg.Cluster != null)
                {
                    if (organisation.Contracts != null)
                    {
                        var removedContracts = dborg.Contracts.Where(p => organisation.Contracts.Select(x => x.ContractNo).Contains(p.ContractNo) == false).ToList();
                        var updatedContracts = dborg.Contracts.Where(p => organisation.Contracts.Select(x => x.ContractNo).Contains(p.ContractNo) == true).ToList();
                        var addedContracts = organisation.Contracts.Where(x => dborg.Contracts.Select(p => p.ContractNo).Contains(x.ContractNo) == false).ToList();

                        foreach (var contract in removedContracts)
                        {
                            DataContext.Instance.Contracts.Remove(contract);
                        }

                        foreach (var contract in addedContracts)
                        {
                            Contract dbcontract = new Contract();
                            dbcontract.Base64File = contract.Base64File;
                            dbcontract.Description = contract.Description;
                            dbcontract.Interval = contract.Interval;
                            dbcontract.IsActive = contract.IsActive;
                            dbcontract.NextInvoicedate = contract.NextInvoicedate;
                            dbcontract.OngoingPayment = contract.OngoingPayment;
                            dbcontract.Organisation = dborg;
                            dbcontract.Payout = contract.Payout;
                            dbcontract.Title = contract.Title;
                            DataContext.Instance.Contracts.Add(dbcontract);
                        }

                        foreach (var contract in updatedContracts)
                        {
                            var dbcontract = DataContext.Instance.Contracts.Where(p => p.ContractNo == contract.ContractNo && p.Organisation == dborg).FirstOrDefault();
                            dbcontract.Base64File = contract.Base64File;
                            dbcontract.Description = contract.Description;
                            dbcontract.Interval = contract.Interval;
                            dbcontract.IsActive = contract.IsActive;
                            dbcontract.NextInvoicedate = contract.NextInvoicedate;
                            dbcontract.OngoingPayment = contract.OngoingPayment;
                            dbcontract.Organisation = dborg;
                            dbcontract.Payout = contract.Payout;
                            dbcontract.Title = contract.Title;
                        }
                    }

                    if (organisation.Invoices != null)
                    {
                        var removedInvoices = dborg.Invoices.Where(p => organisation.Invoices.Select(x => x.Invoiceno).Contains(p.InvoiceNo) == false).ToList();
                        var updatedInvoices = dborg.Invoices.Where(p => organisation.Invoices.Select(x => x.Invoiceno).Contains(p.InvoiceNo) == true).ToList();
                        var addedInvoices = organisation.Invoices.Where(x => dborg.Invoices.Select(p => p.InvoiceNo).Contains(x.Invoiceno) == false).ToList();

                        foreach (var invoice in removedInvoices)
                        {
                            DataContext.Instance.Invoices.Remove(invoice);
                        }

                        foreach (var invoice in addedInvoices)
                        {
                            Invoice dbinvoice = new Invoice();
                            dbinvoice.Amout = invoice.Amout;
                            dbinvoice.Base64File = invoice.Base64File;
                            dbinvoice.Details = invoice.Details;
                            dbinvoice.DueDate = invoice.DueDate;
                            dbinvoice.Invoicedate = invoice.Invoicedate;
                            dbinvoice.Organisation = dborg;
                            dbinvoice.Status = invoice.Status;
                            dbinvoice.Titel = invoice.Titel;
                            DataContext.Instance.Invoices.Add(dbinvoice);
                        }

                        foreach (var invoice in updatedInvoices)
                        {
                            var dbinvoice = DataContext.Instance.Invoices.Where(p => p.InvoiceNo == invoice.InvoiceNo && p.Organisation == dborg).FirstOrDefault();
                            dbinvoice.Amout = invoice.Amout;
                            dbinvoice.Base64File = invoice.Base64File;
                            dbinvoice.Details = invoice.Details;
                            dbinvoice.DueDate = invoice.DueDate;
                            dbinvoice.Invoicedate = invoice.Invoicedate;
                            dbinvoice.Organisation = dborg;
                            dbinvoice.Status = invoice.Status;
                            dbinvoice.Titel = invoice.Titel;
                        }
                    }

                    dborg.Metadata.Clear();
                    foreach (var metadata in organisation.Metadata)
                    {
                        OrganisationMetadata meta = new OrganisationMetadata();
                        meta.Organisation = dborg;
                        meta.Key = metadata.Key;
                        // if (metadata.Scope != scope)
                        // {
                        //     throw new Exception("CRITICAL ERROR: Metadata scope does not match login scope");
                        // }
                        meta.Scope = metadata.Scope;
                        meta.Value = metadata.Value;

                        DataContext.Instance.OrganisationMetadatas.Add(meta);
                    }

                    DataContext.Instance.SaveChanges();
                    return dborg;
                }
            }

            return null;
        }

        /// <summary>
        /// Delete an organisation
        /// </summary>
        /// <param name="id">id of the organisation to delete</param>
        /// <returns>true if the organisation was deleted</returns>
        public static bool Delete(Guid id)
        {
            // TODO: revisionslog, hvem har opdateret (fra hvad, til hvad og hvornår)
            Organisation dborg = DataContext.Instance.Organisations.Where(p => p.Id == id).FirstOrDefault();
            if (dborg != null)
            {
                DataContext.Instance.Organisations.Remove(dborg);
                DataContext.Instance.SaveChanges();
                return true;
            }

            return false;
        }



        ///// <summary>
        ///// Get all organisations on a given cluster
        ///// </summary>
        ///// <param name="id">id of the cluster to find the organisations in</param>
        ///// <returns>list of organisations in the cluster</returns>
        //public static List<Organisation> ReadByCluster(int id)
        //{
        //    List<Organisation> res = new List<Organisation>();
        //    var cluster = DataContext.Instance.Clusters.Where(p => p.ClusterId == id).FirstOrDefault();
        //    if (cluster != null)
        //    {
        //        foreach (var org in cluster.Organisations)
        //        {
        //            res.Add(org);
        //        }
        //    }

        //    return res;
        //}

        /// <summary>
        /// Get a specific organisation
        /// </summary>
        /// <param name="id">id of the organisation to get</param>
        /// <returns>The found organisation</returns>
        public static List<Organisation> GetByIds(Guid[] id)
        {
            var dborg = DataContext.Instance.Organisations.Where(p => id.Contains(p.Id)).ToList();
            return dborg;
        }

        /// <summary>
        /// Convert list of organisationsDTO to organisationsDTO
        /// </summary>
        /// <param name="scope">The scope in which the organisation is converted (only metadata from that scope will be included)</param>
        /// <param name="orgs">Organisations in dto format</param>
        /// <returns>Object with array of organisation DTOs</returns>
        public static Auth.DTO.Organisations ConvertToDTO(string scope, List<Auth.DTO.Organisation> orgs)
        {
            Auth.DTO.Organisations result = new Auth.DTO.Organisations();
            result.Organisation = orgs.ToArray();
            return result;
        }

        /// <summary>
        /// Convert an db organisation to a DTO
        /// </summary>
        /// <param name="scope">The scope in which the organisation is converted (only metadata from that scope will be included)</param>
        /// <param name="dborg">the database representation of the organisation</param>
        /// <returns>DTO representation of an organisation</returns>
        public static Auth.DTO.Organisation ConvertToDTO(string scope, Organisation dborg)
        {
            if (dborg != null)
            {
                Auth.DTO.Organisation organisation = new Auth.DTO.Organisation();
                organisation.Id = dborg.Id;
                organisation.Name = dborg.Name;
                organisation.ParentOrganisation = dborg.ParentOrganisation == null ? Guid.Empty : dborg.ParentOrganisation.Id;

                // we call DataContext.Instance.Contracts instead of the relation, to avoid NULL error
                organisation.Contracts = DataContext.Instance.Contracts.Where(p => p.Organisation.Id == dborg.Id).Select(p => new Auth.DTO.Contract()
                {
                    Base64File = p.Base64File,
                    ContractNo = p.ContractNo,
                    Description = p.Description,
                    Interval = p.Interval,
                    IsActive = p.IsActive,
                    NextInvoicedate = p.NextInvoicedate,
                    OngoingPayment = p.OngoingPayment,
                    Payout = p.Payout,
                    Title = p.Title
                }).ToArray();
                organisation.Invoices = DataContext.Instance.Invoices.Where(p => p.Organisation.Id == dborg.Id).Select(p => new Auth.DTO.Invoice()
                {
                    Amout = p.Amout,
                    Base64File = p.Base64File,
                    Details = p.Details,
                    DueDate = p.DueDate,
                    Invoicedate = p.Invoicedate,
                    Invoiceno = p.InvoiceNo,
                    Status = p.Status,
                    Titel = p.Titel
                }).ToArray();
                organisation.Metadata = DataContext.Instance.OrganisationMetadatas.Where(p => p.Organisation.Id == dborg.Id && p.Scope == scope).Select(p => new Common.DTO.MetaData()
                {
                    Key = p.Key,
                    Scope = p.Scope,
                    Value = p.Value
                }).ToArray();

                organisation.Cluster = Cluster.ConvertToDTO(dborg.Cluster);

                return organisation;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert an db organisation to a DTO without detailed information
        /// </summary>
        /// <param name="scope">The scope in which the organisation is converted (only metadata from that scope will be included)</param>
        /// <param name="dborg">the database representation of the organisation</param>
        /// <returns>DTO representation of an organisation</returns>
        public static Auth.DTO.Organisation ConvertToDTOWoDetails(string scope, Organisation dborg)
        {
            if (dborg != null)
            {
                Auth.DTO.Organisation organisation = new Auth.DTO.Organisation();
                organisation.Id = dborg.Id;
                organisation.Name = dborg.Name;
                organisation.Metadata = DataContext.Instance.OrganisationMetadatas.Where(p => p.Organisation.Id == dborg.Id && p.Scope == scope).Select(p => new Common.DTO.MetaData()
                {
                    Key = p.Key,
                    Scope = p.Scope,
                    Value = p.Value
                }).ToArray();

                organisation.Cluster = null;

                return organisation;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert an db organisation to a DTO
        /// </summary>
        /// <param name="scope">The scope in which the organisation is converted (only metadata from that scope will be included)</param>
        /// <returns>DTO representation of an organisation</returns>
        public Auth.DTO.Organisation ConvertToDTO(string scope)
        {
            return Organisation.ConvertToDTO(scope, this);
        }

        /// <summary>
        /// Defines the database structure for this tabel
        /// </summary>
        /// <param name="modelBuilder">modelbuilder</param>
        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organisation>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Organisation>()
                .HasMany(c => c.Contracts)
                .WithOne(e => e.Organisation)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Organisation>()
                .HasMany(c => c.Invoices)
                .WithOne(e => e.Organisation)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Organisation>()
                .HasMany(c => c.SubOrganisations)
                .WithOne(e => e.ParentOrganisation)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Organisation>()
                .HasMany(c => c.Metadata)
                .WithOne(e => e.Organisation)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}