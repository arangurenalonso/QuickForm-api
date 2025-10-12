using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Users.Domain;

namespace QuickForm.Modules.Users.Application;
public class RegisterPermissionCommandHandler(
        IUnitOfWork _unitOfWork
    ) : ICommandHandler<RegisterPermissionCommand, ResultResponse>
{

    public async Task<ResultT<ResultResponse>> Handle(RegisterPermissionCommand request, CancellationToken cancellationToken)
    {
        List<string> resourcesString = request.Endpoints.SelectMany(x => x.Tags).ToList();


        var resourceaaaaa = await _unitOfWork.Repository<ResourcesDomain>()
                                        .GetAll(false, cancellationToken);
        Console.WriteLine(resourceaaaaa.ToString());

        var resourcesDistinct = resourcesString.Distinct().ToList();
        var resourcesKeyNameResult = resourcesDistinct.Select(x => KeyNameVO.Create(x)).ToList();
        var resourcesKeyNameFailure = resourcesKeyNameResult.Where(x => x.IsFailure).ToList();
        if(resourcesKeyNameFailure.Any())
        {
            var errors = resourcesKeyNameFailure.SelectMany(x => x.Errors.ToList()).ToList();
            return ResultT<ResultResponse>.FailureT(ResultType.BadRequest, errors);
        }

        List<KeyNameVO> resources = resourcesKeyNameResult.Select(x => x.Value).ToList();

        var findResultResources = await _unitOfWork.Repository<ResourcesDomain>().GetByKeyNames(resources,false,null,cancellationToken);

        if (findResultResources.HasMissing)
        {
            var missingResourcesVO = findResultResources.Missing;

            var newResourcesResult = missingResourcesVO.Select(x => ResourcesDomain.Create(x)).ToList();
            var newResourcesFailure = newResourcesResult.Where(x => x.IsFailure).ToList();
            if (newResourcesFailure.Any())
            {
                var errors = newResourcesFailure.SelectMany(x => x.Errors.ToList()).ToList();
                return ResultT<ResultResponse>.FailureT(ResultType.BadRequest, errors);
            }
            var newResources = newResourcesResult.Select(x => x.Value).ToList();
            _unitOfWork.Repository<ResourcesDomain,MasterId>().AddEntity(newResources);
        }



        var confirmTransactionResult = await _unitOfWork.SaveChangesWithResultAsync(GetType().Name, cancellationToken);
        if (confirmTransactionResult.IsFailure)
        {
            return ResultT<ResultResponse>.FailureT(confirmTransactionResult.ResultType, confirmTransactionResult.Errors);
        }

        return ResultResponse.Success("User created successfully.");
    }

}

