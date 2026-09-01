using Xunit;

// Тесты ValidateMajorRevitVersionTests и RemoveRevitApiCopyLocalTests выполняют реальную сборку
// таргета через ProjectInstance.Build(), который использует общий статический BuildManager.
// При параллельном запуске тестовых классов (поведение xUnit по умолчанию) конкурентные
// вызовы Build() падают с "Не удалось завершить операцию, так как уже выполняется сборка."
// Поэтому параллелизм отключён для всей сборки - тестов мало, это не критично.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
