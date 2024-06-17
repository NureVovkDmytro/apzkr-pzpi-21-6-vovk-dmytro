#include "TestDataSource.h"

TestDataSource::TestDataSource()
{
    for(int i = 0; i < 10; i++)
    {
        positions.push_back(Position(30 + i, 20 + i, 0));
    }
}

TestDataSource::~TestDataSource()
{
}

Position TestDataSource::getPosition()
{
    if (lastPositionIndex >= positions.size())
    {
        lastPositionIndex = 0;
    }
    return positions[lastPositionIndex++];
}

void TestDataSource::init()
{
}

void TestDataSource::tick()
{
}