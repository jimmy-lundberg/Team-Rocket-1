﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FootballManager.Admin.Utility;
using FootballManager.Admin.View;
using Domain.Entities;
using Domain.Services;
using FootballManager.Admin.Extensions;
using Domain.Helper_Classes;
using Domain.Value_Objects;
using Match = Domain.Entities.Match;

namespace FootballManager.Admin.ViewModel
{
    public class SeriesScheduleViewModel : ViewModelBase
    {
        private ObservableCollection<Series> seriesCollection;
        private ObservableCollection<Match> matchesBySeriesCollection;
        private Match selectedMatch;

        private SeriesService seriesService;

        private Series selectedSeries;

        private ICommand openCreateSeriesGameProtocolViewCommand;
        private ICommand openSeriesScheduleEditViewCommand;
        private ICommand deleteSelectedSeriesCommand;

        public SeriesScheduleViewModel()
        {
            matchesBySeriesCollection = new ObservableCollection<Match>();

            seriesService = new SeriesService();

            Messenger.Default.Register<Series>(this, OnSeriesObjReceived);
            LoadData();            
        }

        #region Properties
        public ICommand OpenCreateSeriesGameProtocolViewCommand
        {
            get
            {
                if (openCreateSeriesGameProtocolViewCommand == null)
                {
                    openCreateSeriesGameProtocolViewCommand = new RelayCommand(OpenCreateSeriesGameProtocolView);
                }
                return openCreateSeriesGameProtocolViewCommand;
            }
        }

        public ICommand OpenSeriesScheduleEditViewCommand
        {
            get
            {
                if (openSeriesScheduleEditViewCommand == null)
                {
                    openSeriesScheduleEditViewCommand = new RelayCommand(OpenSeriesScheduleEditView);
                }
                return openSeriesScheduleEditViewCommand;
            }
        }

        public ICommand DeleteSelectedSeriesCommand
        {
            get
            {
                if (deleteSelectedSeriesCommand == null)
                {
                    deleteSelectedSeriesCommand = new RelayCommand(DeleteSeries);
                }
                return deleteSelectedSeriesCommand;
            }
        }

        public Series SelectedSeries
        {
            get { return selectedSeries; }
            set
            {
                selectedSeries = value;
                OnPropertyChanged();
                FilterMatchesBySeries();
            }
        }
        #endregion

        public Match SelectedMatch
        {
            get { return selectedMatch; }
            set
            {
                selectedMatch = value;
                OnPropertyChanged();
            }
        }

        #region Collections               
        public ObservableCollection<Series> SeriesCollection
        {
            get { return seriesCollection; }
            set
            {
                seriesCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Match> MatchesBySeriesCollection
        {
            get { return matchesBySeriesCollection; }
            set
            {
                matchesBySeriesCollection = value;
                OnPropertyChanged();
                matchesBySeriesCollection = value;
            }
        }
        #endregion

        #region Methods               
        private void OpenCreateSeriesGameProtocolView(object param)
        {
            var view = new SeriesGameProtocolView();
            if (param != null)
            {
                Messenger.Default.Send<Match>((Match)param);
            }
            view.ShowDialog();
        }

        private void DeleteSeries(object obj)
        {
            if (this.selectedSeries != null)
            {
                this.seriesService.DeleteSeries(this.selectedSeries.Id);
                this.SeriesCollection = seriesService.GetAll().ToObservableCollection();
                this.MatchesBySeriesCollection.Clear();
                if (!(this.SeriesCollection.Count <= 0))
                {
                    this.SelectedSeries = SeriesCollection.ElementAt(0);
                }
                
            }

        }
        

        public void FilterMatchesBySeries()
        {
            if (SelectedSeries != null)
            {
                var t = SelectedSeries.Schedule;

                MatchesBySeriesCollection = t.ToObservableCollection();
            }
            
        }

        private void OpenSeriesScheduleEditView(object obj)
        {
            var view = new SeriesScheduleEditView();
            Messenger.Default.Send<Match>(this.selectedMatch);
            view.ShowDialog();

            this.MatchesBySeriesCollection = this.SelectedSeries.Schedule.ToObservableCollection();
        }

        private void OnSeriesObjReceived(Series serie)
        {
            seriesService.Add(serie);
            seriesService.ScheduleGenerator(serie.Id);
            SeriesCollection = seriesService.GetAll().ToObservableCollection();
        }
        private void LoadData()
        {
            this.seriesCollection = seriesService.GetAll().ToObservableCollection();
        }
        #endregion
    }
}
