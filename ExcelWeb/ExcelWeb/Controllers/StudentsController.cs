﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using ExcelWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelWeb.Controllers
{
    public class StudentsController : Controller
    {
        [HttpGet]
        public IActionResult Index(List<Student> students = null)
        {
            students = students == null ? new List<Student>() : students;
            return View(students);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            var students = this.GetStudentList(file.FileName);
            return Index(students);
        }

        private List<Student> GetStudentList(string fName)
        {
            List<Student> students = new List<Student>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        students.Add(new Student()
                        {
                            Name = reader.GetValue(0).ToString(),
                            Age = reader.GetValue(1).ToString(),
                            Email = reader.GetValue(2).ToString()
                        });
                    }
                }
            }
            return students;
        }
    }
}
