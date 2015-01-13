using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.Data;

namespace StockAnalyzer
{

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      // STEP 1: GET USER INPUT ---------------------------------------------------------
      // - Ticker
      // - StartingDate
      // - EndingDate
      // - Frequency

      // Instantiate aStockTracker object for use during events
      // Default constructor 
      // Consider way to incorporate 4 param constructor instead
      // Can this be moved inside button event?
      public static aStockTracker tracker = new aStockTracker();
      //public aStringSplitter splitter;  
      public static aStringSplitter splitter = new aStringSplitter();      

      // Set ending date to today
      private void datePicker_EndingDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         tracker.EndingDate = DateTime.Now; 
      }

      // Set starting date to at least 10 trading days earlier
      // Assume a month to be safe
      private void datePicker_StartingDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         // TimeSpan span = TimeSpan.FromDays(20);          

         tracker.StartingDate = tracker.EndingDate.AddMonths(-1);
      }

      // Radio buttons for frequency. Defaults to "d" 
      private void radioButton_daily_Checked(object sender, RoutedEventArgs e)
      {
         tracker.Frequency = "d";
      }

     
      // --------------------------------------------------------------------------------

      // Run button click event. This is the main event in the program
      private void button_Run_Click(object sender, RoutedEventArgs e)
      {
         // Set ticker on tracker object
         try
         {
            if (textBox_Ticker.Text != "")
            {
               tracker.Ticker = textBox_Ticker.Text;
            }
            else
            {
               throw new Exception("Invalid ticker");
            }
         }//end try
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message);
            return;
         }//end catch

         tracker.EndingDate = DateTime.Now;
         tracker.StartingDate = tracker.EndingDate.AddMonths(-1);
         tracker.Frequency = "d";



         // STEP 2: GET WEB DATA --------------------------------------------------------
         // Read RTN stock data from Yahoo finance website into string
         try
         {
            // Call buildURL() method from aStockTracker class
            tracker.getWebData();
         }//end try
         catch (WebException ex)
         {
            MessageBox.Show("Web Exception: Please check all inputs and \n" +
            "and make sure you have an internet connection!");
            return;
         }//end catch

         // -----------------------------------------------------------------------------

         // STEP 3: SPLIT THE STRING ----------------------------------------------------
         // Call aStringSplitter to parse raw sting and return DataTable object

         DataTable table = splitter.SplitToDataTable(tracker.RawString);

         // -----------------------------------------------------------------------------

         // STEP 4: PERFORM ANALYSIS ----------------------------------------------------
         // Calculate the ten day avg 

         try
         {
            textBox_TenDayAvg.Text = tracker.calcTenDayAvg(table).ToString();
         }
         catch(Exception ex)
         {
            MessageBox.Show("What are you a FUCKING Idiot!\n" + 
               "\nYou have to pick at least 10 days to get a 10 day average.\n\n" +
               ex.Message); 
         }

         // -----------------------------------------------------------------------------

         
      }//end button run click

      

      /// <summary>
      /// Show new window with DataGrid of downloaded historical stock data
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button1_Click(object sender, RoutedEventArgs e)
      {
         // Pop up new window showing DataGrid
         Window_DataGridView dataGridWindow = new Window_DataGridView();
         dataGridWindow.Show();
      }

      private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         // Consider restarting the whole program from here instead
       
         textBox_Ticker.Text = "";
         textBox_TenDayAvg.Text = ""; 

         // How do I reset this value??? 
         // datePicker_StartingDate 

         
      }

     

     
   }
}
