﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactiveHistory;
using ReactiveHistorySample.Models;

namespace ReactiveHistorySample.ViewModels
{
    public class LineShapeViewModel : IDisposable
    {
        private CompositeDisposable Disposable { get; set; }

        public ReactiveProperty<string> Name { get; set; }
        public ReactiveProperty<PointShapeViewModel> Start { get; set; }
        public ReactiveProperty<PointShapeViewModel> End { get; set; }

        public ReactiveCommand DeleteCommand { get; set; }

        public LineShapeViewModel(LineShape line, IHistory history)
        {
            Disposable = new CompositeDisposable();

            this.Name = line.ToReactivePropertyAsSynchronized(l => l.Name)
                .SetValidateNotifyError(name => string.IsNullOrWhiteSpace(name) ? "Name can not be null or whitespace." : null)
                .AddTo(this.Disposable);

            this.Start = new ReactiveProperty<PointShapeViewModel>(new PointShapeViewModel(line.Start, history))
                .SetValidateNotifyError(start => start == null ? "Point can not be null." : null)
                .AddTo(this.Disposable);

            this.End = new ReactiveProperty<PointShapeViewModel>(new PointShapeViewModel(line.End, history))
                .SetValidateNotifyError(end => end == null ? "Point can not be null." : null)
                .AddTo(this.Disposable);

            this.Name.ObserveWithHistory(name => line.Name = name, line.Name, history).AddTo(this.Disposable);

            this.DeleteCommand = new ReactiveCommand();
            this.DeleteCommand.Subscribe((x) => Delete(line, history)).AddTo(this.Disposable);
        }

        private void Delete(LineShape line, IHistory history)
        {
            if (line.Owner != null && line.Owner is Layer)
            {
                (line.Owner as Layer).Shapes.DeleteWithHistory(line, history);
            }
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}