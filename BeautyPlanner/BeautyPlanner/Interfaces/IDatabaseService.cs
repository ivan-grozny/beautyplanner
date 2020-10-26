using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeautyPlanner.Models;

namespace BeautyPlanner.Interfaces
{
    public interface IDatabaseService
    {
        Task<List<Month>> GetMonthsAsync();
        Task<List<Day>> GetDaysAsync();
        Task<Month> GetCurrentMonthAsync(DateTime date);
        Task<int> SaveMonthAsync(Month month);
        Task<int> DeleteMonthAsync(Month month);
        Task<List<Appointment>> GetAppointmentsForMonthAsync(int monthId);
        Task<List<Day>> GetDaysForMonthAsync(int monthId);
        Task SaveAppointmentsAsync(IEnumerable<Appointment> appointments, int dayId, int monthId);
        Task<int> SaveAppointmentAsync(Appointment appointment);
        Task<int> DeleteAppointmentAsync(Appointment appointment);
    }
}