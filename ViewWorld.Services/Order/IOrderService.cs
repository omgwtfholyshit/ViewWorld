﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Interfaces;
using ViewWorld.Core.Models.BusinessModels;

namespace ViewWorld.Services.Order
{
    public interface IOrderService:ICRUDable<BusinessOrder>
    {
        Task<GetOneResult<BusinessOrder>> RetrieveOrdersById(string id);
        Task<GetManyResult<BusinessOrder>> RetrieveOrderByOrderId(string orderId);
        Task<GetListResult<BusinessOrder>> RetrieveOrdersByKeyword(string keyword, string salesId);
        Task<Result> AssignOrderToSales(string salesId, string orderId);
        Task<Result> UpdateOrderById(string salesId, BusinessOrder order, bool authorized);
        Task<Result> CreatePriceGapOrder(string orderId, double price, string salesId, bool authorized);
        Task<long> CountOrders(string userId, bool deletedOrders = false);
        Task<GetListResult<BusinessOrder>> GetOrder(OrderStatus status, ProductType type,string userId, string id);
    }
}
