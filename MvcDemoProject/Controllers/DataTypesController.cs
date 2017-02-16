using MvcDemoProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace MvcDemoProject.Controllers
{
    public class DataTypesController : Controller
    {

        public ActionResult Index()
        {

            List<ContractorType> types = new List<ContractorType>();

            using (var context = new DbEntities())
            {

                types = context.ContractorType.ToList();

            }

            return View(types);

        }

        public ActionResult Add()
        {

            var model = GetModelFromJson();

            try
            {
                using (var context = new DbEntities())
                {
                    ContractorType NewContratorType = new ContractorType
                    {
                        Name = model.name
                    };

                    context.ContractorType.Add(NewContratorType);
                    context.SaveChanges();
                }

            }
            catch
            {
                return RedirectToAction("Index", "DataTypes");
            }

            return RedirectToAction("Index", "DataTypes");

        }


        public ActionResult Delete(int id)
        {
            try
            {
                ContractorType contractorType = new ContractorType() { keyId = id };
                using (var context = new DbEntities())
                {
                    context.ContractorType.Attach(contractorType);
                    context.ContractorType.Remove(contractorType);
                    context.SaveChanges();
                }

                return RedirectToAction("Index", "DataTypes");

            }
            catch 
            {
                return RedirectToAction("Index", "DataTypes");
            }
        }

        private types GetModelFromJson()
        {
            var resolveRequest = HttpContext.Request;
            types model = new types();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonString = new StreamReader(resolveRequest.InputStream).ReadToEnd();
            if (jsonString != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                model = (types)serializer.Deserialize(jsonString, typeof(types));
            }

            return model;

        }


    }

    class types
    {
        [JsonProperty("name")]
        public string name { get; set; }
    }
}