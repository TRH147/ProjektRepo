# Fix Three Build Errors in UserService

## What & Why
Three build errors in UserService.cs are preventing the project from compiling.

## Done looks like
- The project builds with no errors
- The app starts up successfully

## Out of scope
- Any other refactoring or logic changes
- Fixing the CS8602 nullable warnings in ThreadsController (warnings only, not errors)

## Tasks
1. **Fix TryRemove lambda parameter conflict** — On lines 614 and 631, the `ContinueWith` lambda uses `_` as its parameter name, which conflicts with the `out _` discard in the same expression. Rename the lambda parameter (e.g. to `t`) on both lines so they read `ContinueWith(t => _usernameCache.TryRemove(..., out _))` and `ContinueWith(t => _emailCache.TryRemove(..., out _))`.

2. **Fix wrong property name on Post** — On line 675, change `p.ForumThreadId` to `p.ThreadId` to match the actual property name defined on the `Post` model.

## Relevant files
- `Services/UserService.cs:610-635`
- `Services/UserService.cs:670-680`
- `Model/Post.cs`
