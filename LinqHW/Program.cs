using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistinctLinq
{
    public class Program
    {
        static void Main(string[] args)
        {
            string cs = ConfigurationManager.ConnectionStrings["cs"].ToString();
            string sqlQuery = "Select * from Employees ";
            string sqlEmpTerritories = "Select * from EmployeeTerritories";
            string sqlTerritories = "Select * from Territories";
            string sqlOrder = "Select * from Orders";
            string sqlOrderDit = "Select * from [Order Details]";
            string sqlProducts = "Select * from Products";
            string sqlSupplier = "Select * from Suppliers";
            SqlDataAdapter adapterFirst = new SqlDataAdapter(sqlQuery, cs);
            SqlDataAdapter adapterSecond = new SqlDataAdapter(sqlEmpTerritories, cs);
            SqlDataAdapter adapterThird = new SqlDataAdapter(sqlTerritories, cs);
            SqlDataAdapter adapterFourth = new SqlDataAdapter(sqlOrder, cs);
            SqlDataAdapter adapterFifth = new SqlDataAdapter(sqlOrderDit, cs);
            SqlDataAdapter adaptersixth = new SqlDataAdapter(sqlProducts, cs);
            SqlDataAdapter adapterSeventh = new SqlDataAdapter(sqlSupplier, cs);
            DataSet dsFirst = new DataSet();
            adapterFirst.Fill(dsFirst, "Employees");
            adapterSecond.Fill(dsFirst, "EmployeeTerritories");
            adapterThird.Fill(dsFirst, "Territories");
            adapterFourth.Fill(dsFirst, "Order");
            adapterFifth.Fill(dsFirst, "Order Details");
            adaptersixth.Fill(dsFirst, "Products");
            adapterSeventh.Fill(dsFirst, "Suppliers");

            //   Поиск всех сотрудников возраст, которых от 60 до 100
            var ageGap = from element in dsFirst.Tables[0].AsEnumerable()
                         let age = (DateTime.Now - (DateTime)element["BirthDate"]).Days / 365
                         where (age > 60 && age < 100)
                         select new
                         {
                             emplId = element["EmployeeID"],
                             name = element["FirstName"],
                             age = age
                         };

            //Поиск сотрудников по странам
            var empCountry = from emp in dsFirst.Tables[0].AsEnumerable()
                             from id in dsFirst.Tables[1].AsEnumerable()
                             from territories in dsFirst.Tables[2].AsEnumerable()
                             where emp.Field<Int32>("EmployeeID") == id.Field<Int32>("EmployeeID") && id.Field<String>("TerritoryID") == territories.Field<String>("TerritoryID")
                             select new
                             {
                                 id = emp["EmployeeID"],
                                 empName = emp["FirstName"],
                                 country = territories["TerritoryDescription"]
                             };

            //Для каждого сотрудника вывести начальника и подчиненного

            var employee = from emp in dsFirst.Tables[0].AsEnumerable()
                           select new
                           {
                               employee = emp["FirstName"],
                               empsChief = emp["ReportsTo"]
                           };


            var supplier = from order in dsFirst.Tables[3].AsEnumerable()
                           from orderdit in dsFirst.Tables[4].AsEnumerable()
                           from products in dsFirst.Tables[5].AsEnumerable()
                           from suppliers in dsFirst.Tables[6].AsEnumerable()
                           where order.Field<Int32>("OrderID") == orderdit.Field<Int32>("OrderID") && orderdit.Field<Int32>("ProductID") == products.Field<Int32>("ProductID")
                           && products.Field<Int32>("SupplierID") == suppliers.Field<Int32>("SupplierID")
                           select new
                           {
                               order = order["OrderID"],
                               suppliers = suppliers["SupplierID"],
                               supplierName = suppliers["CompanyName"]
                           };


            foreach (var item in supplier)
            {
                Console.WriteLine(item);

            }

            Console.ReadLine();
        }
    }
}
