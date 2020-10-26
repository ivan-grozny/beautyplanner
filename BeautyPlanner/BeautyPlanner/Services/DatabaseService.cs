using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeautyPlanner.Helpers;
using BeautyPlanner.Interfaces;
using BeautyPlanner.Models;
using SQLite;

namespace BeautyPlanner.Services
{
    public class DatabaseService : IDatabaseService
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });
       
        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        private async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!IsTableCreated(typeof(Month).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Month)).ConfigureAwait(false);
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Day)).ConfigureAwait(false);
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Appointment)).ConfigureAwait(false);
                    var monthNumber = 1;
                    foreach (var kvp in Constants.MonthsDictionary)
                    {
                        await CreateMonth(monthNumber, DateTime.Now.Year);
                        monthNumber++;
                    }
                }
                initialized = true;
            }
        }

        public Task<List<Month>> GetMonthsAsync()
        {
            return Database.Table<Month>().ToListAsync();
        }

        public Task<List<Day>> GetDaysAsync()
        {
            return Database.Table<Day>().ToListAsync();
        }

        public async Task<Month> GetCurrentMonthAsync(DateTime date)
        {
            var month = await Database.Table<Month>().Where(m => m.Number == date.Month && m.Year == date.Year).FirstOrDefaultAsync();

             if (month == null)
             {
                 month = await CreateMonth(date.Month, date.Year);
             }

             return month;
        }

        public Task<List<Appointment>> GetAppointmentsForMonthAsync(int monthId)
        {
            return Database.Table<Appointment>().Where(a => a.MonthId == monthId).ToListAsync();
        }

        public Task<List<Day>> GetDaysForMonthAsync(int monthId)
        {
            return Database.Table<Day>().Where(a => a.MonthId == monthId).ToListAsync();
        }

        public async Task<int> SaveMonthAsync(Month month)
        {
            if (month.Id != 0)
            {
                await Database.UpdateAsync(month);
                return month.Id;
            }
            else
            {
                await Database.InsertAsync(month);
                return month.Id;
            }
        }

        public async Task<int> SaveDayAsync(Day day)
        {
            if (day.Id != 0)
            {
                await Database.UpdateAsync(day);
                return day.Id;
            }
            else
            {
                await Database.InsertAsync(day);
                return day.Id;
            }
        }

        public async Task<int> SaveAppointmentAsync(Appointment appointment)
        {
            if (appointment.Id != 0)
            {
                await Database.UpdateAsync(appointment);
                return appointment.Id;
            }
            else
            {
                await Database.InsertAsync(appointment);
                return appointment.Id;
            }
        }

        public Task<int> DeleteMonthAsync(Month month)
        {
            return Database.DeleteAsync(month);
        }

        public Task<int> DeleteAppointmentAsync(Appointment appointment)
        {
            return Database.DeleteAsync(appointment);
        }

        public async Task SaveAppointmentsAsync(IEnumerable<Appointment> appointments, int dayId, int monthId)
        {
            foreach (var appointment in appointments)
            {
                appointment.DayId = dayId;
                appointment.MonthId = monthId;
                await SaveAppointmentAsync(appointment);
            }
        }

        private bool IsTableCreated(string tableName)
        {
            try
            {
                var tableInfo = Database.GetConnection().GetTableInfo(tableName);
                if (tableInfo.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        private async Task<Month> CreateMonth(int monthNumber, int year)
        {
            var month = new Month
            {
                Name = Constants.MonthsNamesDictionary[monthNumber],
                Number = monthNumber,
                Year = year
            };

            var monthId = await SaveMonthAsync(month);
            var dayNumber = 1;
            for (var v = 0; v < Constants.MonthsDictionary[month.Name]; v++)
            {
                var day = new Day
                {
                    Date = new DateTime(year, monthNumber, dayNumber),
                    MonthId = monthId
                };
                dayNumber++;
                await SaveDayAsync(day);
            }

            return month;
        }

    }

}
