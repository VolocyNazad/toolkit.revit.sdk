// The ValidateMajorRevitVersionTests and RemoveRevitApiCopyLocalTests tests perform a real target
// build via ProjectInstance.Build(), which uses the shared static BuildManager.
// When test classes run in parallel (xUnit's default behavior), concurrent
// Build() calls fail with "Cannot complete the operation because a build is already in progress."
// Parallelism is therefore disabled for the whole assembly - there aren't many tests, so this isn't a problem.
[assembly: Xunit.v3.Parallelization(Mode = Xunit.Sdk.ParallelMode.None)]
