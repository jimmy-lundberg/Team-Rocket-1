﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FootballManager.Admin.Utility;
using FootballManager.Admin.View;
using MaterialDesignThemes.Wpf;
using Domain.Entities;
using System.Collections.ObjectModel;
using Domain.Services;
using FootballManager.Admin.Extensions;
using System.Collections;
using Domain.Value_Objects;

namespace FootballManager.Admin.ViewModel
{
    public class SeriesViewModel : ViewModelBase
    {
        private ObservableCollection<Team> availableTeams;
        private ObservableCollection<Team> teamsToAddToSeries;
        private List<int> numberOfTeamsList;
        private TeamService teamService;
        private SeriesService seriesService;
        private Team selectedTeam;
        private string seriesName;
        private int matchDuration;
        private string searchText;
        private int selectedNumberOfTeams;

        public SeriesViewModel()
        {
            this.teamsToAddToSeries = new ObservableCollection<Team>();
            this.numberOfTeamsList = new List<int>();
            this.teamService = new TeamService();
            this.seriesService = new SeriesService();
            this.AddTeamCommand = new RelayCommand(AddTeam);
            this.DeleteTeamCommand = new RelayCommand(DeleteTeam);
            this.AddSeriesCommand = new RelayCommand(AddSeriesTeam);
            LoadData();
        }

        public ICommand DeleteTeamCommand { get; }

        public ICommand AddTeamCommand { get; }

        public ICommand AddSeriesCommand { get; }

        public ObservableCollection<Team> AvailableTeams
        {
            get { return availableTeams; }
            set
            {
                availableTeams = value;
                OnPropertyChanged();                
            }
        }

        public ObservableCollection<Team> TeamsToAddToSeries
        {
            get { return teamsToAddToSeries; }
            set
            {
                teamsToAddToSeries = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged();
                AvailableTeams = teamService.GetAll().
                    Where(x => x.Name.Value.ToLower().
                    Contains(searchText.ToLower())).ToObservableCollection();
            }
        }

        public int SelectedNumberOfTeams
        {
            get { return selectedNumberOfTeams; }
            set
            {
                if (selectedNumberOfTeams != value)
                {
                    selectedNumberOfTeams = value;
                    OnPropertyChanged();
                }
            }
        }

        public Team SelectedTeam
        {
            get { return selectedTeam; }
            set
            {
                selectedTeam = value;
                OnPropertyChanged();
            }
        }

        public string SeriesName
        {
            get { return seriesName; }
            set
            {
                seriesName = value; 
                OnPropertyChanged();
            }
        }

        public int MatchDuration
        {
            get { return matchDuration; }
            set
            {
                matchDuration = value;
                OnPropertyChanged();
            }
        }

        public List<int> NumberOfTeamsList
        {
            get { return numberOfTeamsList; }
            set
            {
                numberOfTeamsList = value;
                OnPropertyChanged();
            }
        }

        private void AddTeam(object obj)
        {
            if (selectedTeam != null && !(teamsToAddToSeries.Contains(selectedTeam)))
            {
                teamsToAddToSeries.Add(selectedTeam);
                availableTeams.Remove(selectedTeam);
            }
        }

        private void DeleteTeam(object obj)
        {
            if (selectedTeam != null && !(availableTeams.Contains(selectedTeam)))
            {
                availableTeams.Add(selectedTeam);
                teamsToAddToSeries.Remove(selectedTeam);
                
            }

        }

        private void AddSeriesTeam(object obj)
        {
            var seriesSeriesName = new SeriesName(this.seriesName);
            var seriesNumberOfTeams = new NumberOfTeams(this.SelectedNumberOfTeams);
            var seriesMatchDuration = new MatchDuration(new TimeSpan(0, this.matchDuration, 0));

            var seriesToAdd = new Series(seriesMatchDuration, seriesNumberOfTeams, seriesSeriesName);

            foreach (var team in teamsToAddToSeries)
            {
                seriesToAdd.TeamIds.Add(team.Id);
            }
            seriesService.Add(seriesToAdd);
            seriesService.ScheduleGenerator(seriesToAdd.Id);
            teamsToAddToSeries.Clear();
            this.SeriesName = "";
            this.MatchDuration = 0;
            this.AvailableTeams = teamService.GetAll().ToObservableCollection();
        }

        public void LoadData()
        {
            this.AvailableTeams = teamService.GetAll().ToObservableCollection();

            var teamLengths = teamService.GetAll().Count();
            for (int i = 0; i <= teamLengths; i++)
            {
                if (IsEven(i) && i != 0 && i != 2)
                {
                    NumberOfTeamsList.Add(i);
                }
            }

        }

        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }

    }
}
