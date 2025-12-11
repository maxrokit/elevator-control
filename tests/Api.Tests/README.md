# Elevator Control API - Unit Tests

This project contains comprehensive unit tests for the Elevator Control API, specifically testing the core domain logic for elevator floor management.

## Test Coverage

### ElevatorTests.cs
Tests for the `Elevator` entity covering all business logic methods:

#### AddFloorDestination Tests (3 tests)
- Adding new floor destinations from inside the elevator
- Preventing duplicate floor destinations
- Managing multiple floor destinations

#### AddFloorCall Tests (5 tests)
- Adding up/down calls from outside the elevator
- Merging both directions for the same floor
- Preserving existing direction calls
- Handling multiple floors with different directions

#### GetNextFloor Tests (10 tests)
- Determining next floor with no requests
- Moving up and selecting nearest floor above
- Switching from up to down when no floors above
- Moving down and selecting nearest floor below
- Switching from down to up when no floors below
- Combining internal destinations and external calls
- Handling mixed calls and destinations
- Ignoring duplicates between calls and destinations
- Managing both direction calls on same floor
- Complete up-then-down sequencing

#### MoveToFloor Tests (7 tests)
- Updating current floor position
- Removing internal destination on arrival
- Clearing up calls when moving up
- Clearing down calls when moving down
- Removing floor call completely when only one direction exists
- Handling floors with no calls or destinations
- Direction-specific call clearing

#### Integration Scenarios (5 tests)
- Servicing multiple floor calls in proper order
- Handling dynamic requests while elevator is moving
- Elevator at top floor with downward requests
- Multiple passengers entering and exiting
- Handling opposite direction calls after current direction completes

## Running the Tests

From the solution root:

```powershell
dotnet test tests\Api.Tests\Api.Tests.csproj
```

From the test project directory:

```powershell
cd tests\Api.Tests
dotnet test
```

With verbose output:

```powershell
dotnet test tests\Api.Tests\Api.Tests.csproj --verbosity normal
```

## Test Framework

- **xUnit** 2.9.0 - Testing framework
- **Microsoft.NET.Test.Sdk** 17.11.0 - Test SDK
- **.NET 10.0** - Target framework

## Key Testing Patterns

1. **Arrange-Act-Assert**: All tests follow this clear pattern
2. **Integration Scenarios**: Complex multi-step scenarios test real-world usage
3. **Direction Awareness**: Tests verify that floor calls are cleared only in the direction of travel
4. **Thread Safety**: Tests verify lock-based synchronization works correctly
5. **Edge Cases**: Tests cover empty requests, duplicate requests, and direction switching

## Test Results

All 30 tests pass successfully, validating:
- ✅ Internal floor destination requests
- ✅ External floor calls with direction (up/down)
- ✅ Next floor calculation with direction changes
- ✅ Floor call clearing based on travel direction
- ✅ Complex multi-passenger scenarios
