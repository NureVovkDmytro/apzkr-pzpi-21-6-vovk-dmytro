#pragma once

struct Position
{
    double northing;
    double easting;
    float compass;

    Position(double northing, double easting, float compass)
    {
        this->northing = northing;
        this->easting = easting;
        this->compass = compass;
    }

    Position()
    {
        this->northing = 0;
        this->easting = 0;
        this->compass = 0;
    }
};
