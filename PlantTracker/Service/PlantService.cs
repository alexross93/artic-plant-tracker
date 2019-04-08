/*
 * Alexander Ross - 040873561
 * CST8333 Assignment 04
 * Professor Stan Pieda
 * March 15, 2019
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlantTracker.Models;

namespace PlantTracker.Service
{
    /// <summary>  
    ///  This class is the service class
    ///  which handles useful services such
    ///  adding, editing, deleting and reloading
    ///  the plant list
    /// </summary> 
    public class PlantService
    {
        //plant data read from dataset
        public static List<PlantModel> plants = readData();
        public static List<SelectListItem> listItems = getListItems();
        public static List<SelectListItem> listOrder = getListOrder();

        /// <summary>  
        ///  This method is save method
        ///  which handles saving a new plant list
        ///  in place of the old list
        /// </summary> 
        /// /// <param name="plants">plants list to overwrite the old list</param>
        public static void savePlants(List<PlantModel> plants)
        {
            PlantService.plants = plants;
        }

        /// <summary>  
        ///  This creates and returns the dropdown list
        ///  items for the plant properties so that the user
        ///  can select which column to sort by
        /// </summary> 
        public static List<SelectListItem> getListItems()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            listItems.Add(new SelectListItem { Text = "Id", Value = "Id", Selected = true });
            listItems.Add(new SelectListItem { Text = "Species", Value = "Species", Selected = false });
            listItems.Add(new SelectListItem { Text = "Year", Value = "Year", Selected = false });
            listItems.Add(new SelectListItem { Text = "JulianDayOfTheYear", Value = "JulianDayOfTheYear", Selected = false });
            listItems.Add(new SelectListItem { Text = "PlantId", Value = "PlantId", Selected = false });
            listItems.Add(new SelectListItem { Text = "NumBuds", Value = "NumBuds", Selected = false });
            listItems.Add(new SelectListItem { Text = "NumFlowers", Value = "NumFlowers", Selected = false });
            listItems.Add(new SelectListItem { Text = "NumFlowersToReachMaturity", Value = "NumFlowersToReachMaturity", Selected = false });
            listItems.Add(new SelectListItem { Text = "ObserverInitials", Value = "ObserverInitials", Selected = false });
            listItems.Add(new SelectListItem { Text = "ObserverComments", Value = "ObserverComments", Selected = false });

            return listItems;
        }

        /// <summary>  
        ///  This creates and returns the dropdown list
        ///  so that the user can select whether the want the list
        ///  ordered in asceding or descending order
        /// </summary> 
        public static List<SelectListItem> getListOrder()
        {
            List<SelectListItem> listOrder = new List<SelectListItem>();

            listOrder.Add(new SelectListItem { Text = "Ascending", Value = "Ascending", Selected = true });
            listOrder.Add(new SelectListItem { Text = "Descending", Value = "Descending", Selected = false });

            return listOrder;
        }

        /// <summary>  
        ///  This creates and returns the dropdown list
        ///  items for the plant properties so that the user
        ///  can select which column to sort by
        /// </summary> 
        public static List<PlantModel> sortPlants(List<PlantModel> plants, String selection, String order)
        {

            foreach (SelectListItem s in listItems)
            {
                //reset selected item in dropdown list of plant columns
                if (s.Selected == true)
                    s.Selected = false;

                //set the selected item to show in view once sort is complete
                if (s.Text.Equals(selection))
                    s.Selected = true;
            }

            foreach (SelectListItem s in listOrder)
            {
                //reset selected item in dropdown list of order
                if (s.Selected == true)
                    s.Selected = false;

                //set the selected order to show in view once sort is complete
                if (s.Text.Equals(order))
                    s.Selected = true;
            }

            if (order.Equals("Ascending"))
            {
                // use Linq to sort list and reflection to dynamically get method name
                plants = plants.OrderBy(o => o.GetType().GetMethod("get" + selection).Invoke(o, null)).ToList();
            }
            else
            {
                // use Linq to sort descending and reflection to dynamically get method name
                plants = plants.OrderByDescending(o => o.GetType().GetMethod("get" + selection).Invoke(o, null)).ToList();
            }

            return plants;
        }

        /// <summary>  
        ///  This method is the remove method which
        ///  takes in a plant, finds it in the list 
        ///  and rmeoves it
        /// </summary> 
        /// /// <param name="plant">plant to be removed</param>
        public static void removePlant(PlantModel plant)
        {

            for (int i=0; i < PlantService.plants.Count; i++)
            {
                if (PlantService.plants[i].Id == plant.Id)
                {
                    PlantService.plants.Remove(plants[i]);
                    deleteFromDb(plant);
                }

            }
        }

        /// <summary>  
        ///  This method is the add method which
        ///  takes in a plant and inserts it into the list
        /// </summary> 
        /// <param name="plant">plant to be added</param>
        public static void addPlant(PlantModel plant)
        {
            //make sure the plants id is unique and add the plant
            plant.Id = PlantService.plants.Count+1;
            insertIntoDb(plant);
            PlantService.plants = readData();
            //PlantService.plants.Add(plant);
        }

        /// <summary>  
        ///  This method is the update method which
        ///  passes the plant to the update method for
        ///  the database and also resets the in-memory
        ///  list once the database is updated
        /// </summary> 
        /// <param name="plant">plant to be added</param>
        public static void updatePlant(PlantModel plant)
        {
            updateIntoDb(plant);
            PlantService.plants = readData();

        }

        /// <summary>  
        ///  This method is the reload method which
        ///  re-reads in the dataset and overwrites 
        ///  the old list
        /// </summary> 
        public static void reload()
        {
            foreach (SelectListItem s in listItems)
            {
                if (s.Selected == true)
                    s.Selected = false;

                if (s.Text.Equals("Id"))
                    s.Selected = true;
            }

            foreach (SelectListItem s in listOrder)
            {
                if (s.Selected == true)
                    s.Selected = false;

                if (s.Text.Equals("Ascending"))
                    s.Selected = true;
            }
            plants = readData();
        }


        /// <summary>  
        ///  This method is the read data method which
        ///  uses File IO to read from the local csv file
        /// </summary> 
        public static List<PlantModel> readData()
        {
            List<string> listColumns = new List<string>();
            List<PlantModel> listData = new List<PlantModel>();

            try
            {
                // build the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = "Server=localhost\\MSSQLSERVER01;Database=master;Trusted_Connection=True;";

                // connect to the database
                Console.Write("Attempting to connect to SQL Server.\n");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // open the connection
                    connection.Open();
                    Console.WriteLine("\nSuccessfully connected to SQL Server.\n");

                    // read from the database table
                    Console.WriteLine("Attempting to read data from the table in the database.\n");
                    String sql = "SELECT * FROM [CST8333].[dbo].[PlantData]";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlantModel newPlant = new PlantModel(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                                    reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7),
                                    reader.GetString(8), reader.GetString(9));

                                listData.Add(newPlant);

                            }
                        }
                    }
                }
                
            }

            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return listData;
        }


        /// <summary>  
        ///  This method is used to connect to SQL server
        ///  and insert a row into the db
        /// </summary> 
        /// <param name="plant">row to be inserted</param>
        public static void insertIntoDb(PlantModel plant)
        {
            try
            {

                // build the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = "Server=localhost\\MSSQLSERVER01;Database=master;Trusted_Connection=True;";

                // connect to the database
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // open the connection
                    connection.Open();

                    String sql; 
                    StringBuilder sb = new StringBuilder();
                    sb.Clear();
                    sb.Append("INSERT [CST8333].[dbo].[PlantData] (Species, Year, JulianDayOfTheYear," +
                        "PlantId, NumBuds, NumFlowers, NumFlowersToReachMaturity, ObserverInitials, ObserverComments) ");
                    sb.Append("VALUES (@species, @year, @juliandayoftheyear, @plantid, @numbuds, @numflowers, @numflowerstoreachmaturity, @observerinitials, @observercomments);");
                    sql = sb.ToString();

                    //protect against null values for strings that throw sql exceptions by setting them to empty strings instead
                    if (plant.Species == null)          plant.Species = "";
                    if (plant.ObserverInitials == null) plant.ObserverInitials = "";
                    if (plant.ObserverComments == null) plant.ObserverComments = "";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@species", plant.Species);
                        command.Parameters.AddWithValue("@year", plant.Year);
                        command.Parameters.AddWithValue("@juliandayoftheyear", plant.JulianDayOfTheYear);
                        command.Parameters.AddWithValue("@plantid", plant.PlantId);
                        command.Parameters.AddWithValue("@numbuds", plant.NumBuds);
                        command.Parameters.AddWithValue("@numflowers", plant.NumFlowers);
                        command.Parameters.AddWithValue("@numflowerstoreachmaturity", plant.NumFlowersToReachMaturity);
                        command.Parameters.AddWithValue("@observerinitials", plant.ObserverInitials);
                        command.Parameters.AddWithValue("@observercomments", plant.ObserverComments);
                        int rowsAffected = command.ExecuteNonQuery();
                    } 
                }

            }

            catch (SqlException e)
            {
                Debug.Write(e.ToString());
            }

        }


        /// <summary>  
        ///  This method is used to connect to SQL server
        ///  and update a row in the db
        /// </summary> 
        /// <param name="plant">plant that will update specified row</param>
        public static void updateIntoDb(PlantModel plant)
        {
            try
            {
                // build the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = "Server=localhost\\MSSQLSERVER01;Database=master;Trusted_Connection=True;";

                // connect to the database
                Console.Write("Attempting to connect to SQL Server.\n");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // open the connection
                    connection.Open();

                    String sql;
                    StringBuilder sb = new StringBuilder();
                    sb.Clear();
                    sb.Append("UPDATE [CST8333].[dbo].[PlantData] SET Species = @species, Year = @year, JulianDayOfTheYear = @juliandayoftheyear,"
                        + "PlantId = @plantid, NumBuds = @numbuds, NumFlowers = @numflowers, NumFlowersToReachMaturity = @numflowerstoreachmaturity,"
                        + "ObserverInitials = @observerinitials, ObserverComments = @observercomments WHERE Id = @id");
                    sql = sb.ToString();

                    //protect against null values for strings that throw sql exceptions by setting them to empty strings instead
                    if (plant.Species == null)           plant.Species = "";
                    if (plant.ObserverInitials == null)  plant.ObserverInitials = "";
                    if (plant.ObserverComments == null)  plant.ObserverComments = "";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", plant.Id);
                        command.Parameters.AddWithValue("@species", plant.Species);
                        command.Parameters.AddWithValue("@year", plant.Year);
                        command.Parameters.AddWithValue("@juliandayoftheyear", plant.JulianDayOfTheYear);
                        command.Parameters.AddWithValue("@plantid", plant.PlantId);
                        command.Parameters.AddWithValue("@numbuds", plant.NumBuds);
                        command.Parameters.AddWithValue("@numflowers", plant.NumFlowers);
                        command.Parameters.AddWithValue("@numflowerstoreachmaturity", plant.NumFlowersToReachMaturity);
                        command.Parameters.AddWithValue("@observerinitials", plant.ObserverInitials);
                        command.Parameters.AddWithValue("@observercomments", plant.ObserverComments);
                        int rowsAffected = command.ExecuteNonQuery();
                    }

                }

            }

            catch (SqlException e)
            {
                Debug.Write(e.ToString());
            }


        }

        /// <summary>  
        ///  This method is used to connect to SQL server
        ///  and delete a row in the db
        /// </summary> 
        /// <param name="plant">row to be inserted</param>
        public static void deleteFromDb(PlantModel plant)
        {
            try
            {

                // build the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = "Server=localhost\\MSSQLSERVER01;Database=master;Trusted_Connection=True;";

                // connect to the database
                Console.Write("Attempting to connect to SQL Server.\n");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // open the connection
                    connection.Open();

                    String sql;
                    StringBuilder sb = new StringBuilder();
                    sb.Clear();
                    sb.Append("DELETE FROM [CST8333].[dbo].[PlantData] WHERE Id = @id;");
                    sql = sb.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", plant.Id);
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException e)
            {
                Debug.Write(e.ToString());
            }


        }
    }
}
