﻿using Ontologia.API.Domain.Models;
using Ontologia.API.Domain.Persistence.Repositories;
using Ontologia.API.Domain.Services;
using Ontologia.API.Domain.Services.Communications;

namespace Ontologia.API.Services
{
    public class PlantDiseaseService : IPlantDiseaseService
    {
        private readonly IPlantDiseaseRepository _plantDiseaseRepository;
        private readonly IUserConceptPlantDiseaseRepository _userConceptPlantDiseaseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlantDiseaseService(IPlantDiseaseRepository plantDiseaseRepository, IUnitOfWork unitOfWork, IUserConceptPlantDiseaseRepository userConceptPlantDiseaseRepository)
        {
            _plantDiseaseRepository = plantDiseaseRepository;
            _userConceptPlantDiseaseRepository = userConceptPlantDiseaseRepository;
            _unitOfWork = unitOfWork;
        }

        // General Methods
        public async Task<PlantDiseaseResponse> SaveAsync(PlantDisease plantDisease)
        {
            try
            {
                await _plantDiseaseRepository.AddAsync(plantDisease);
                await _unitOfWork.CompleteAsync();

                return new PlantDiseaseResponse(plantDisease);
            }
            catch (Exception ex)
            {
                return new PlantDiseaseResponse($"An error while saving PlantDisease:{ex.Message}");
            }
        }

        public async Task<IEnumerable<PlantDisease>> ListAsync()
        {
            return await _plantDiseaseRepository.ListAsync();
        }

        public async Task<PlantDiseaseResponse> GetById(Guid plantDiseaseId)
        {
            var existingPlantDisease = await _plantDiseaseRepository.GetById(plantDiseaseId);
            if (existingPlantDisease == null)
                return new PlantDiseaseResponse("PlantDisease Not Found");
            return new PlantDiseaseResponse(existingPlantDisease);
        }

        public async Task<PlantDiseaseResponse> Update(Guid plantDiseaseId, PlantDisease plantDisease)
        {
            var existingPlantDisease = await _plantDiseaseRepository.GetById(plantDiseaseId);
            if (existingPlantDisease == null)
                return new PlantDiseaseResponse("PlantDisease Not Found");

            existingPlantDisease.PlantDiseaseName = plantDisease.PlantDiseaseName;
            existingPlantDisease.PlantDiseaseDescription = plantDisease.PlantDiseaseDescription;
            existingPlantDisease.IsActive = plantDisease.IsActive;
            existingPlantDisease.CreatedOn = plantDisease.CreatedOn;
            existingPlantDisease.ModifiedOn = plantDisease.ModifiedOn;

            try
            {
                _plantDiseaseRepository.Update(existingPlantDisease);
                await _unitOfWork.CompleteAsync();
                return new PlantDiseaseResponse(existingPlantDisease);
            }
            catch (Exception ex)
            {
                return new PlantDiseaseResponse($"An error while updating PlantDisease: {ex.Message}");
            }
        }

        public async Task<PlantDiseaseResponse> Delete(Guid plantDiseaseId)
        {
            var existingPlantDisease = await _plantDiseaseRepository.GetById(plantDiseaseId);
            if (existingPlantDisease == null)
                return new PlantDiseaseResponse("PlantDisease Not Found");
            try
            {
                _plantDiseaseRepository.Remove(existingPlantDisease);
                await _unitOfWork.CompleteAsync();
                return new PlantDiseaseResponse(existingPlantDisease);
            }
            catch (Exception ex)
            {
                return new PlantDiseaseResponse($"An error ocurrend while deleting PlantDisease: {ex.Message}");
            }
        }

        // Methods for CategoryDisease Entity
        public async Task<IEnumerable<PlantDisease>> ListByCategoryDiseaseId(Guid categoryDiseaseId)
        {
            return await _plantDiseaseRepository.ListByCategoryDiseaseIdAsync(categoryDiseaseId);
        }

        public async Task<PlantDiseaseResponse> AssingPlantDiseaseToCategoryDisease(Guid categoryDiseaseId, Guid plantDiseaseId)
        {
            try
            {
                await _plantDiseaseRepository.AssingPlantDiseaseToCategoryDisease(categoryDiseaseId, plantDiseaseId);
                await _unitOfWork.CompleteAsync();
                PlantDisease plantDisease = await _plantDiseaseRepository.GetById(plantDiseaseId);
                return new PlantDiseaseResponse(plantDisease);
            }
            catch (Exception ex)
            {
                return new PlantDiseaseResponse($"An error ocurrend while assigning PlantDisease to conceptType: {ex.Message}");
            }
        }

        public async Task<PlantDiseaseResponse> UnassingPlantDiseaseToCategoryDisease(Guid categoryDiseaseId, Guid plantDiseaseId)
        {
            try
            {
                await _plantDiseaseRepository.UnassingPlantDiseaseToCategoryDisease(categoryDiseaseId, plantDiseaseId);
                await _unitOfWork.CompleteAsync();
                PlantDisease plantDisease = await _plantDiseaseRepository.GetById(plantDiseaseId);
                return new PlantDiseaseResponse(plantDisease);
            }
            catch (Exception ex)
            {
                return new PlantDiseaseResponse($"An error ocurrend while unassigning PlantDisease to conceptType: {ex.Message}");
            }
        }

        // Methods for UserConceptPlantDisease Entity
        public async Task<IEnumerable<PlantDisease>> ListByUserConceptIdAsync(Guid userConceptId)
        {
            var userConceptsPlantDiseases = await _userConceptPlantDiseaseRepository.ListByUserConceptIdAsync(userConceptId);
            var plantDiseases = userConceptsPlantDiseases.Select(pt => pt.PlantDisease).ToList();
            return plantDiseases;
        }
    }
}
