using FluentScheduler;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using System;

namespace Signals.Aspects.BackgroundProcessing.FluentScheduler.Configuration
{
    internal static class ConfigurationExtensions
    {
        internal static void Configure(this Registry registry, ISyncTask task, RecurrencePatternConfiguration configuration)
        {
            switch (configuration)
	        {
		        case DailyRecurrencePatternConfiguration dailyConfiguration:
			        var dailySchedule = registry.Schedule(() => task.Execute());
					Schedule(dailySchedule, dailyConfiguration); 
					break;
		        case MonthlyNamedRecurrencePatternConfiguration monthlyNamedConfiguration:
			        var monthlyNamedSchedule = registry.Schedule(() => task.Execute());
					Schedule(monthlyNamedSchedule, monthlyNamedConfiguration);
			        break;
		        case MonthlyRecurrencePatternConfiguration monthlyConfiguration:
			        var monthlySchedule = registry.Schedule(() => task.Execute());
					Schedule(monthlySchedule, monthlyConfiguration);
			        break;
		        case TimePartRecurrencePatternConfiguration timePartConfiguration:
			        var timePartSchedule = registry.Schedule(() => task.Execute());
					Schedule(timePartSchedule, timePartConfiguration);
			        break;
		        case WeekendRecurrencePatternConfiguration weekendConfiguration:
			        var saturdayWeekendDaySchedule = registry.Schedule(() => task.Execute());
			        var sundayWeekendDaySchedule = registry.Schedule(() => task.Execute());
					Schedule(saturdayWeekendDaySchedule, sundayWeekendDaySchedule, weekendConfiguration);
			        break;
		        case WeeklyRecurrencePatternConfiguration weeklyConfiguration:
			        var weeklySchedule = registry.Schedule(() => task.Execute());
					Schedule(weeklySchedule, weeklyConfiguration);
			        break;
		        case WorkdayRecurrencePatternConfiguration workdayConfiguration:
			        var workdaySchedule = registry.Schedule(() => task.Execute());
					Schedule(workdaySchedule, workdayConfiguration);
			        break;
	        }
        }

		/// <summary>
		/// Configure a daily schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    DailyRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Days()
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Days()
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
	    }

		/// <summary>
		/// Configure a monthly named schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    MonthlyNamedRecurrencePatternConfiguration configuration)
		{
			MonthUnit monthSchedule;

		    if (configuration.RunNow)
		    {
			    monthSchedule = schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Months();
			}
		    else
		    {
			    monthSchedule = schedule
				    .ToRunEvery(configuration.Value)
				    .Months();
			}

			MonthOnDayOfWeekUnit orderedSchedule = null;

		    switch (configuration.Order)
		    {
			    case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.First:
				    orderedSchedule = monthSchedule.OnTheFirst(configuration.Day);
				    break;
			    case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Second:
				    orderedSchedule = monthSchedule.OnTheSecond(configuration.Day);
				    break;
			    case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Third:
				    orderedSchedule = monthSchedule.OnTheThird(configuration.Day);
				    break;
			    case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Fourth:
				    orderedSchedule = monthSchedule.OnTheFourth(configuration.Day);
				    break;
			    case MonthlyNamedRecurrencePatternConfiguration.DayInMonth.Last:
				    orderedSchedule = monthSchedule.OnTheLast(configuration.Day);
				    break;
		    }

		    orderedSchedule?.At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
	    }

		/// <summary>
		/// Configure a monthly schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    MonthlyRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Months()
				    .On(configuration.Day)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Months()
				    .On(configuration.Day)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
	    }

		/// <summary>
		/// Configure a time part schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    TimePartRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Seconds();
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Seconds();
			}
	    }

		/// <summary>
		/// Schedule a weekend task
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="secondSchedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    Schedule secondSchedule,
			WeekendRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Weeks()
				    .On(DayOfWeek.Saturday)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Weeks()
				    .On(DayOfWeek.Saturday)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}

		    if (configuration.RunNow)
		    {
			    secondSchedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Weeks()
				    .On(DayOfWeek.Sunday)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    secondSchedule
				    .ToRunEvery(configuration.Value)
				    .Weeks()
				    .On(DayOfWeek.Sunday)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
	    }

		/// <summary>
		/// Configure a weekly schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    WeeklyRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Weeks()
				    .On(configuration.Day)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Weeks()
				    .On(configuration.Day)
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
	    }

		/// <summary>
		/// Configure a workdays schedule
		/// </summary>
		/// <param name="schedule"></param>
		/// <param name="configuration"></param>
		private static void Schedule(
		    Schedule schedule,
		    WorkdayRecurrencePatternConfiguration configuration)
	    {
		    if (configuration.RunNow)
		    {
			    schedule
					.ToRunNow()
				    .AndEvery(configuration.Value)
				    .Weekdays()
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
		    else
		    {
			    schedule
				    .ToRunEvery(configuration.Value)
				    .Weekdays()
				    .At(configuration.TimePart.Hours, configuration.TimePart.Minutes);
			}
	    }
	}
}