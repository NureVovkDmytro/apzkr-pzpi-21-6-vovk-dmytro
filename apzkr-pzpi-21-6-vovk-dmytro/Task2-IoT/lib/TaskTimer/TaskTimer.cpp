#include "TaskTimer.h"

TaskTimer::TaskTimer()
{
    this->tasks = std::vector<TimerTask>();
}

TaskTimer::~TaskTimer()
{
    this->tasks.clear();
}

void TaskTimer::add(std::function<void()> callback, unsigned long interval)
{
    TimerTask task;
    task.callback = callback;
    task.interval = interval;
    task.lastRun = 0;
    this->tasks.push_back(task);
}

void TaskTimer::tick()
{
    for (TimerTask &task : this->tasks)
    {
        if (millis() - task.lastRun > task.interval)
        {
            task.lastRun = millis();
            task.callback();
        }
    }
}