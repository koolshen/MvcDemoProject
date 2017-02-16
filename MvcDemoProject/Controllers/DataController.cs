using MvcDemoProject.Models;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.Web.Script.Serialization;

namespace MvcDemoProject.Controllers
{
    public class DataController : Controller
    {

        public ActionResult GetList(int? id)
        {
            try
            {
                if (id == null)
                {
                    using (var context = new DbEntities())
                    {
                        var Contractors = context.Contractors.Select(c => new
                        {
                            c.keyId,
                            c.Name,
                            c.INN,
                            c.ContractorTypeId,
                            c.Phone
                        });

                        var ContractorTypes = context.ContractorType.Select(c => new
                        {
                            c.keyId,
                            c.Name
                        });

                        return Json(new { contractors = Contractors.ToList(), contractorTypes = ContractorTypes.ToList() }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    using (var context = new DbEntities())
                    {
                        var Contractors = context.Contractors.Select(c => new
                        {
                            c.keyId,
                            c.Name,
                            c.INN,
                            c.ContractorTypeId,
                            c.Phone
                        }).Where(c => c.keyId == id);
                        return Json(new { contractors = Contractors.ToList() }, JsonRequestBehavior.AllowGet);
                    }

                }
            } catch (Exception ex)
            {
                return GenerateJsonResponse("error : + " + ex);
            }


        }


        public ActionResult Update()
        {
            try
            {
                var model = GetModelFromJson();

                using (var context = new DbEntities())
                {
                    var contractorToUpdate = context.Contractors.SingleOrDefault(c => c.keyId == model.keyId);
                    if (contractorToUpdate != null)
                    {
                        contractorToUpdate.Name = model.Name;
                        contractorToUpdate.INN = model.INN;
                        contractorToUpdate.ContractorTypeId = model.ContractorTypeId;
                        contractorToUpdate.Phone = model.Phone;
                        context.SaveChanges();
                    }
                    else
                    {
                        return GenerateJsonResponse("Возникла ошибка при обновлении! Не найдена запрашиваемый контрагент не найден");
                    }
                }
            } catch (Exception ex)
            {
                return GenerateJsonResponse("Возникла ошибка при обновлении! " + ex.Message);
            }


            return GenerateJsonResponse("Обновление произведено успешно!");

        }

        public ActionResult Add()
        {
            try {
                var model = GetModelFromJson();
                using (var context = new DbEntities())
                {
                    Contractors NewContrator = new Contractors
                    {
                        Name = model.Name,
                        INN = model.INN,
                        ContractorTypeId = model.ContractorTypeId,
                        Phone = model.Phone
                    };

                    context.Contractors.Add(NewContrator);
                    context.SaveChanges();
                }

            } catch (Exception ex)
            {
                return GenerateJsonResponse("Ошибка при добавлении данных: +" + ex.Message);
            }

            return GenerateJsonResponse("Добавление произведено успешно!");

        }


        public ActionResult Delete(int id)
        {
            try
            {
                Contractors Contractor = new Contractors() { keyId = id };
                using (var context = new DbEntities())
                {
                    context.Contractors.Attach(Contractor);
                    context.Contractors.Remove(Contractor);
                    context.SaveChanges();
                }

                return GenerateJsonResponse("Удаление произведено успешно!");

            } catch (Exception ex)
            {
                return GenerateJsonResponse("error : + " + ex);
            }
        }

        private JsonResult GenerateJsonResponse(string text)
        {
            return Json(new { status = text }, JsonRequestBehavior.AllowGet);
        }

        private Contractors GetModelFromJson()
        {
            var resolveRequest = HttpContext.Request;
            Contractors model = new Contractors();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonString = new StreamReader(resolveRequest.InputStream).ReadToEnd();
            if (jsonString != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                model = (Contractors)serializer.Deserialize(jsonString, typeof(Contractors));
            }

            return model;
            
        }

    }
}