/*
 * Alexander Ross - 040873561
 * CST8333 Assignment 04
 * Professor Stan Pieda
 * March 15, 2019
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PlantTracker.Models;
using PlantTracker.Service;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlantTracker.Controllers
{
    /// <summary>  
    ///  This class is the home controller
    ///  which handles redirects between all pages and 
    ///  also calls the service in order to update the
    ///  dataset and view
    /// </summary> 
    public class HomeController : Controller
    {
        private List<PlantModel> plants = Service.PlantService.plants;
        private List<SelectListItem> listItems = Service.PlantService.listItems;
        private List<SelectListItem> listOrder = Service.PlantService.listOrder;


        /// <summary>  
        ///  empty default constructor
        /// </summary> 
        public HomeController()
        {
        }


        /// <summary>  
        ///  this is the home page that sends the dataset info to the view
        ///  as well as passing the two dropdown lists for sorting to the view
        /// </summary> 
        public IActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            ViewData["Message"] = "List of Plants";
            ViewData["listItems"] = listItems;
            ViewData["listOrder"] = listOrder;
           
            return View(plants);
        }

        /// <summary>  
        ///  here we use the plant service to so sort the plants
        /// </summary> 
        /// <param name="id">Id of the plant model to be edited </param>
        [HttpPost]
        public IActionResult Index(int id)
        {
            ViewData["Title"] = "Home Page";
            ViewData["Message"] = "List of Plants";

            //get the selected values from the dropdown lists and save them
            String selectedValues = Request.Form["dropDownList"].ToString(); //this will get selected valu
            var values = selectedValues.Split(',');

            //set them to variables
            String selection = values.ElementAt(0);
            String order = values.ElementAt(1);

            plants = PlantService.sortPlants(plants, selection, order);

            ViewData["listItems"] = listItems;
            ViewData["listOrder"] = listOrder;

            return View(plants);
        }

        /// <summary>  
        ///  this is the inital edit page method that displays the editable fields
        ///  in the view
        /// </summary> 
        /// <param name="id">Id of the plant model to be edited </param>
        public IActionResult Edit(int? id)
        {
            ViewData["Message"] = "Editing Plant #" + id;

            PlantModel plant = new PlantModel(1, "Plant", 24, 1, 1, 1, 1, 1, "AR", "");

            foreach (PlantModel m in plants)
            {
                if (m.Id == id)
                {
                    plant = m;
                }
            }

            return View(plant);
        }

        /// <summary>  
        ///  this is the edit page method that is fired after the save button is 
        ///  clicked. Once the plant is identified we save it using the serivce
        ///  and redirect back to the home page
        /// </summary> 
        /// <param name="plant">Plant model that is passed from the view </param>
        [HttpPost]
        public IActionResult Edit(PlantModel plant)
        {

            for (int i = 0; i < plants.Count; i++)
            {
                if (plants[i].Id == plant.Id)
                {
                    plants[i] = plant;
                    PlantService.updatePlant(plant);
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>  
        ///  this is the initial delete page method before confirming to delete
        /// </summary> 
        /// <param name="id">Id of the plant we wish to delete</param>
        public IActionResult Delete(int id)
        {
            ViewData["Title"] = "Delete";
            ViewData["Message"] = "Are you sure you want to delete Plant #" + id +"?";

            PlantModel plant = new PlantModel(1, "Plant", 24, 1, 1, 1, 1, 1, "AR", "");

            foreach (PlantModel m in plants)
            {
                if (m.Id == id)
                {
                    plant = m;
                }
            }

            return View(plant);
        }

        /// <summary>  
        ///  this is the delete page method that is triggered after the confirm button is 
        ///  clicked. Once the plant is identified we remove it using the serivce
        ///  and redirect back to the home page
        /// </summary> 
        /// <param name="plant">Plant model that is passed from the view </param>
        [HttpPost]
        public IActionResult Delete(PlantModel plant)
        {

            for (int i = 0; i < plants.Count; i++)
            {
                if (plants[i].Id == plant.Id)
                {
                    //use the service to remove the plant from the list
                    PlantService.removePlant(plant);
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>  
        ///  this is the initial reload confirmation page method before
        ///  confirming to reload
        /// </summary> 
        /// <param name="id">id param that is not used</param>
        public IActionResult Reload(int id)
        {

            ViewData["Message"] = "Are you sure you want to reload the dataset?";
 
            return View();
        }

        /// <summary>  
        ///  this is the reload method that is triggered once the confirm button
        ///  is pressed. Using the service we reload the dataset and redirect back 
        ///  to the home page
        /// </summary> 
        [HttpPost]
        public IActionResult Reload()
        {
            //use the service to reload the list with the orginal dataset
            PlantService.reload();

            return RedirectToAction("Index");
        }

        /// <summary>  
        ///  this is used for the initial add method page before pressing add
        /// </summary> 
        /// <param name="id">id of the plant we willl add</param>
        public IActionResult Add(int id)
        {
            ViewData["Message"] = "Add a new plant";

            return View();
        }

        /// <summary>  
        ///  this is the add method that is triggered upon pressing add
        ///  Using the serivce we add it to the plant list and redirect
        ///  back to the home page
        /// </summary> 
        /// <param name="plant">this is the plant that will be added</param>
        [HttpPost]
        public IActionResult Add(PlantModel plant)
        {
            //use the serivce to add the plant to the list
            Service.PlantService.addPlant(plant);

            return RedirectToAction("Index");
        }
    }
}
