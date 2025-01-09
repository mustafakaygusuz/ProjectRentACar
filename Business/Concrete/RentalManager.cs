using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants.Messages;
using CorePackagesGeneral.Aspects.Performance;
using CorePackagesGeneral.Aspects.Transaction;
using CorePackagesGeneral.Utilities.Business;
using CorePackagesGeneral.Utilities.Results.Abstract;
using CorePackagesGeneral.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;
        ICarDal _carDal;

        public RentalManager(IRentalDal rentalDal, ICarDal carDal)
        {
            _rentalDal = rentalDal;
            _carDal = carDal;
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        [TransactionScopeAspect]
        public IResult AddRental(Rental car)
        {
            _rentalDal.Add(car);
            return new SuccessResult(Messages.RentAlAdded);
        }

        public List<int> CalculateTotalPrice(DateTime rentDate, DateTime returnDate, int carId)
        {
            List<int> totalAmount = new List<int>();
            var dateDifference = (returnDate - rentDate).Days;
            //var datesOfDifference = dateDifference.Days;
            var dailyCarPrice = decimal.ToInt32(_carDal.Get(c => c.Id == carId).DailyPrice);

            var totalPrice = dateDifference * dailyCarPrice;

            totalAmount.Add(dateDifference);
            totalAmount.Add(totalPrice);


            return totalAmount;
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        [TransactionScopeAspect]
        public IResult DeleteRental(Rental car)
        {
            _rentalDal.Delete(car);
            return new SuccessResult(Messages.RentAlDeleted);
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentAlListed);
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        public IDataResult<List<Rental>> GetById(int rentalId)
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(r => r.Id == rentalId));
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        public IDataResult<List<RentalDetailsDto>> GetRentalDetails()
        {
            return new SuccessDataResult<List<RentalDetailsDto>>(_rentalDal.GetRentalDetails());
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        public IResult IsCarAvaible(int carId, DateTime startDate, DateTime endDate)
        {
            IResult result = BusinessRules.Run(IsCarAvaibleForRent(carId, startDate, endDate));
            if (result != null)
            {
                return new ErrorResult(Messages.RentACarNotAvailable);
            }
            return new SuccessResult(Messages.RentACarAvailable);
        }

        public IResult IsCarAvaible(int carId)
        {
            throw new NotImplementedException();
        }

        [SecuredOperation("Admin,Moderator,NormalUser")]
        [PerformanceAspect(15)]
        [TransactionScopeAspect]
        public IResult UpdateRental(Rental car)
        {
            _rentalDal.Update(car);
            return new SuccessResult(Messages.RentAlUptaded);
        }

        private IResult IsCarAvaibleForRent(int carId, DateTime startDate, DateTime endDate)
        {
            var result = _rentalDal.GetAll(r => r.CarId == carId && r.CreatedDate > startDate && r.CreatedDate < endDate).Any();
            if (result)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }
    }
}
