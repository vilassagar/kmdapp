using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Data.Helpers;
namespace KMDRecociliationApp.Data.Repositories
{
    public class MainHeaderRepo<T> : BaseRepo
    {
        ApplicationDbContext _context;
        public MainHeaderRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public T Add(T t, string email = "", string givenName = "", string comment = "")
        {
            try
            {
                foreach (var property in t.GetType().GetProperties())
                {
                    switch (property.Name.ToUpper())
                    {
                        case "CREATEDAT":
                        case "UPDATEDAT":
                            {
                                property.SetValue(t, DateTime.Now);
                                break;
                            }
                        case "CREATEDBY":
                        case "UPDATEDBY":
                            {
                                property.SetValue(t, UserId);
                                break;
                            }
                        default:
                            break;
                    }
                }
                _context.Add(t);
                int retcount = _context.SaveChanges(UserId, email, givenName, comment);
                if (retcount > 0)
                    return t;
                else
                    return default;
            }
            catch (Exception ex)
            {
                Log.Fatal("Error in Add " + ex);
                throw;
            }


        }
        public T Add(T t)
        {
            //try
            //{
                foreach (var property in t.GetType().GetProperties())
                {
                    switch (property.Name.ToUpper())
                    {
                        case "CREATEDAT":
                        case "UPDATEDAT":
                            {
                                property.SetValue(t, DateTime.Now);
                                break;
                            }
                        case "CREATEDBY":
                        case "UPDATEDBY":
                            {
                                property.SetValue(t, UserId);
                                break;
                            }
                        default:
                            break;
                    }
                }
                _context.Add(t);
                int retcount = _context.SaveChanges();
                if (retcount > 0)
                    return t;
                else
                    return default;
            //}
            //catch (Exception ex)
            //{
            //    Log.Fatal("Error in Add " + ex);
            //    throw;
            //}


        }
        public T Update(T t)
        {
            try
            {

                _context.Update(t);
                int retcount = _context.SaveChanges();
                if (retcount > 0)
                    return t;
                else
                    return default;
            }
            catch (Exception ex)
            {
                Log.Fatal("Error in Update " + ex);
                throw;
            }

        }

        public T Update(T t, string email = "", string givenName = "", string comment = "")
        {
            try
            {
                foreach (var property in t.GetType().GetProperties())
                {
                    switch (property.Name.ToUpper())
                    {

                        case "UPDATEDAT":
                            {
                                property.SetValue(t, DateTime.Now);
                                break;
                            }
                        case "UPDATEDBY":
                            {
                                property.SetValue(t, UserId);
                                break;
                            }
                        default:
                            break;
                    }
                }
                _context.Update(t);
                int retcount = _context.SaveChanges(UserId, email, givenName, comment);
                if (retcount > 0)
                    return t;
                else
                    return default;
            }
            catch (Exception ex)
            {
                Log.Fatal("Error in Add " + ex);
                throw;
            }

        }
        public T Delete(T t, string email = "", string givenName = "", string comment = "")
        {
            try
            {
                foreach (var property in t.GetType().GetProperties())
                {
                    switch (property.Name.ToUpper())
                    {
                        case "ISACTIVE":
                            {
                                property.SetValue(t, false);
                                break;
                            }
                        case "UPDATEDAT":
                            {
                                property.SetValue(t, DateTime.Now);
                                break;
                            }
                        case "UPDATEDBY":
                            {
                                property.SetValue(t, UserId);
                                break;
                            }
                        default:
                            break;
                    }
                }
                _context.Update(t);
                int retcount = _context.SaveChanges(UserId, email, givenName, comment);
                if (retcount > 0)
                    return t;
                else
                    return default;
            }
            catch (Exception ex)
            {
                Log.Fatal("Error in Delete " + ex);
                throw;
            }

        }

      
    }
}
