#pragma once
#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include <vector>

struct TimerTask
{
    std::function<void()> callback;
    unsigned long interval;
    unsigned long lastRun;
};

class TaskTimer
{
private:
    static TimerTask emptyTask;
    std::vector<TimerTask> tasks;
public:
    TaskTimer();
    ~TaskTimer();

    void add(std::function<void()> callback, unsigned long interval);
    void tick();
};