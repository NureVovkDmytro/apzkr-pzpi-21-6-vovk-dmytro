#include "DataSource.h"
#include "Position.h"
#include <vector>

using namespace std;

class TestDataSource : public DataSource
{
private:
    vector<Position> positions;
    int lastPositionIndex;

public:
    TestDataSource();
    ~TestDataSource();

    void init();
    void tick();
    Position getPosition();
};