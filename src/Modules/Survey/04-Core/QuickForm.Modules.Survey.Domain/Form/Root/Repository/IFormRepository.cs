﻿namespace QuickForm.Modules.Survey.Domain;

public interface IFormRepository
{
    Task<FormDomain?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<QuestionDomain>> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(FormDomain form);
}
