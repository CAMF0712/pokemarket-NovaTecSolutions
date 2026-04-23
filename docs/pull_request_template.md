# Pull Request Template

## What does this PR do?
Brief description of the changes introduced in this pull request.

Explain:
- What problem it solves
- What functionality was added or modified
- Any important technical decisions

---

## Related Issue / Azure Ticket
Reference the related issue or work item.

Example:
Closes #123  
Azure Ticket: AZ-456

---

## Type of Change
Select the type of change introduced by this PR.

- [ ] New feature
- [ ] Bug fix
- [ ] Refactor / code improvement
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Security fix

---

## How to Test
Describe the steps needed to validate this change.

Example:

1. Start the backend service
2. Call endpoint `/api/auction`
3. Verify that the auction winner is correctly calculated

---

## Screenshots / API Examples (if applicable)
Add screenshots, logs or API responses if this PR changes UI or API behavior.

---

## Breaking Changes
- [ ] No breaking changes
- [ ] Yes (describe below)

If yes, explain what changed and how to migrate.

---

## Checklist Before Merging

- [ ] Code compiles and runs locally
- [ ] Unit tests added or updated if necessary
- [ ] Integration tests added if this affects API behavior
- [ ] Documentation updated if required
- [ ] No sensitive data or credentials committed
- [ ] Code reviewed by at least one team member

---

## Product Impact

- [ ] Does this require analytics tracking?
- [ ] Will this be part of a product update?

If yes, briefly describe the update:

> Example: Adds auction closing validation to guarantee a single winner.

---

## Reviewer
@mention the responsible reviewer or team.

Example:

Reviewer: @backend-team
