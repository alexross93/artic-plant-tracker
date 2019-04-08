/*
 * Alexander Ross - 040873561
 * CST8333 Assignment 04
 * Professor Stan Pieda
 * March 15, 2019
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlantTracker.Models
{
    /// <summary>  
    ///  This class is the plant model class
    ///  which stores all the relevant data
    ///  for a plant
    /// </summary> 
    public class PlantModel : IComparable
    {
        int id;
        String species;
        int year;
        int julianDayOfTheYear;
        int plantId;
        int numBuds;
        int numFlowers;
        int numFlowersToReachMaturity;
        String observerInitials;
        String observerComments;

        /// <summary>  
        ///  empty constructor
        /// </summary> 
        public PlantModel()
        {
        }

        /// <summary>  
        ///  constructor that takes in all the
        ///  relevant data for the plant and sets
        ///  it as its own values
        /// </summary> 
        public PlantModel(int id, String species, int year, int julianDayOfTheYear, int plantId, int numBuds, int numFlowers,
            int numFlowersToReachMaturity, String observerInitials, String observerComments)
        {
            this.id = id;
            this.Species = species;
            this.Year = year;
            this.JulianDayOfTheYear = julianDayOfTheYear;
            this.PlantId = plantId;
            this.NumBuds = numBuds;
            this.NumFlowers = numFlowers;
            this.NumFlowersToReachMaturity = numFlowersToReachMaturity;
            this.ObserverInitials = observerInitials;
            this.ObserverComments = observerComments;
        }

        /// <summary>  
        ///  compare method that is implemented from the comparable interface
        /// </summary>
        /// <param name="obj">object to be compared</param>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            PlantModel otherPlant = obj as PlantModel;
            if (otherPlant != null)
                return this.NumFlowers.CompareTo(otherPlant.NumFlowers);
            else
                throw new ArgumentException("Object is not a a Plant");
        }

        /// <summary>  
        ///  auto-generated getters and setters for the plant
        /// </summary> 
        public string Species { get => species; set => species = value; }
        public int Year { get => year; set => year = value; }
        public int JulianDayOfTheYear { get => julianDayOfTheYear; set => julianDayOfTheYear = value; }
        public int PlantId { get => plantId; set => plantId = value; }
        public int NumBuds { get => numBuds; set => numBuds = value; }
        public int NumFlowers { get => numFlowers; set => numFlowers = value; }
        public int NumFlowersToReachMaturity { get => numFlowersToReachMaturity; set => numFlowersToReachMaturity = value; }
        public string ObserverInitials { get => observerInitials; set => observerInitials = value; }
        public string ObserverComments { get => observerComments; set => observerComments = value; }
        public int Id { get => id; set => id = value; }

        /// <summary>  
        ///  manually created getters in order to use reflection when sorting
        /// </summary> 
        public int getId() { return this.Id; }
        public string getSpecies() { return this.Species; }
        public int getYear() { return this.Year; }
        public int getJulianDayOfTheYear() { return this.JulianDayOfTheYear; }
        public int getPlantId() { return this.PlantId; }
        public int getNumBuds() { return this.NumBuds; }
        public int getNumFlowers() { return this.NumFlowers; }
        public int getNumFlowersToReachMaturity() { return this.NumFlowersToReachMaturity; }
        public string getObserverInitials() { return this.ObserverInitials; }
        public string getObserverComments() { return this.ObserverComments; }

    }
}






