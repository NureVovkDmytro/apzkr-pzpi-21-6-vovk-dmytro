#pragma once
#include "Position.h"

class DataSource
{
public:
    virtual void init() = 0;
    virtual void tick() = 0;

    virtual Position getPosition() = 0;
};
