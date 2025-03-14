using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteErasee.Models;
using Microsoft.EntityFrameworkCore;

namespace WriteErasee.ViewModels
{
	public class EditOrderViewModel : ViewModelBase
	{
		User? _currentUser;
		public List<Status> Statuses { get => _statuses; set => this.RaiseAndSetIfChanged(ref _statuses, value); }
		List<Status> _statuses;
		public Order Order { get => _order; set => this.RaiseAndSetIfChanged(ref _order, value); }
		Order _order;

		public EditOrderViewModel(User? user, Order order)
		{
			_currentUser = user;
			Statuses = MainWindowViewModel.myConnection.Statuses.ToList();
			Order = order;
		}

		public void Exit() => MainWindowViewModel.Instance.PageContent = new ListOrders(_currentUser);

		/// <summary>
		/// Изменения статуса заказа и даты доставки
		/// </summary>
		/// <returns></returns>
		public async Task Save()
		{
			try
			{
				MainWindowViewModel.myConnection.Orders.Update(Order);
				MainWindowViewModel.myConnection.SaveChanges();
				await MessageBoxManager.GetMessageBoxStandard("Сохранено", "Данные заказа были изменены", ButtonEnum.Ok).ShowAsync();
				Exit();
			}
			catch
			{
				await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Проверьте правильность заполнения полей и попробуйте еще раз", ButtonEnum.Ok).ShowAsync();
			}
		}
	}
}
