using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;

namespace FreeDriverScout.Routine
{
    public class TaskManager
    {

        #region Public Methods

        public static Task GetTaskByName(string taskName)
        {
            Task task = null;
            try
            {
                TaskService service = new TaskService();
                task = service.FindTask(taskName, true);
                
            }
            catch { }

            return task;
        }

        public static bool IsTaskScheduled(string taskName)
        {
            Task task = GetTaskByName(taskName);
            if (task != null)
                return true;
            else
                return false;
        
        }

        public static string GetTaskDescription(string taskName)
        {
            string taskDescription = "";
            Task task = GetTaskByName(taskName);
            if (task != null)
            {
                foreach (Trigger trg in task.Definition.Triggers)
                {
                    taskDescription += trg.ToString() + ". ";
                }
            }
            if (taskDescription.IndexOf(", starting") > 0)
                return taskDescription.Substring(0, (taskDescription.Length - taskDescription.IndexOf(", starting")) + 1);
            else
                return taskDescription;
        
        }

        public static void DeleteTask(string taskName)
        {
            Task task = null;
            try
            {
                TaskService service = new TaskService();
                task = service.FindTask(taskName, true);

                if (task != null)
                    service.RootFolder.DeleteTask(taskName);
            }
            catch { }
        
        
        }

        public static void UpdateTaskStatus(string taskName, bool isEnabled)
        {
            try
            {
                TaskService service = new TaskService();
                Task task = service.FindTask(taskName, true);

                if (task != null)
                {
                    task.Enabled = isEnabled;
                    task.RegisterChanges();
                }
            }
            catch{}
        }

        public static void CreateDefaultTask(string taskName, bool isEnabled)
        {
            try
            {
                DeleteTask(taskName);

                TaskService service = new TaskService();
                TaskDefinition td = service.NewTask();
                
                td.Settings.Enabled = isEnabled;
                td.RegistrationInfo.Description = "Freemium Driver Scan";

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction(Environment.CurrentDirectory + "\\1Click.exe", null, null));

                WeeklyTrigger mTrigger = new WeeklyTrigger();
                mTrigger.DaysOfWeek = DaysOfTheWeek.Friday;
                mTrigger.StartBoundary = DateTime.Today.AddHours(12);
               
                mTrigger.Repetition.StopAtDurationEnd = false;
                td.Triggers.Add(mTrigger);
                // Register the task in the root folder
                service.RootFolder.RegisterTaskDefinition(taskName, td);

            }
            catch { }
         
        }


        #endregion
    }
}
