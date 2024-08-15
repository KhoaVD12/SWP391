﻿using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IGiftRepo:IGenericRepo<Gift>
    {
        Task<IEnumerable<Gift>> GetAllGifts();
        Task<Gift> GetGiftById(int id);
        Task CreateGift(Gift gift);
        Task<bool> DeleteGift(int id);
        Task UpdateGift(int id, Gift gift);
    }
}
