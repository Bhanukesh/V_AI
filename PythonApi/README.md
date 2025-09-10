# Vida AI Analytics Engine - Python FastAPI Implementation

A FastAPI-based analytics service for restaurant performance management, providing statistical correlation analysis and revenue forecasting capabilities.

## Features

- **Correlation Analysis**: Calculate statistical correlations between operational metrics and revenue
- **Revenue Forecasting**: Generate predictive models with confidence intervals using linear trend analysis
- **Health Monitoring**: Built-in health check endpoint for service monitoring
- **CORS Enabled**: Cross-origin requests supported for web integration
- **Auto-generated Documentation**: Comprehensive API documentation at `/docs` and `/redoc`

## Installation

```bash
pip install -r requirements.txt
```

## Running the Application

```bash
python main.py
```

Or with uvicorn:

```bash
uvicorn main:app --reload --host 0.0.0.0 --port 8000
```

The API will be available at `http://localhost:8000`
- Swagger documentation: `http://localhost:8000/docs`
- ReDoc documentation: `http://localhost:8000/redoc`

## API Endpoints

### Health Check
- `GET /health` - Service health status

### Analytics Endpoints
- `POST /analytics/correlation` - Calculate correlations between operational metrics and revenue
- `POST /analytics/forecast` - Generate revenue forecasts with confidence intervals

## API Usage Examples

### Correlation Analysis
```bash
curl -X POST "http://localhost:8000/analytics/correlation" \
  -H "Content-Type: application/json" \
  -d '{
    "restaurant_id": 1,
    "metrics": ["prep_time", "table_turnover", "order_accuracy"],
    "correlation_type": "pearson"
  }'
```

### Revenue Forecasting
```bash
curl -X POST "http://localhost:8000/analytics/forecast" \
  -H "Content-Type: application/json" \
  -d '{
    "restaurant_id": 1,
    "historical_data": [
      {"date": "2024-01-01", "revenue": 1500.00},
      {"date": "2024-01-02", "revenue": 1650.00}
    ],
    "forecast_days": 30
  }'
```

## Project Structure

```
PythonApi/
├── main.py                    # FastAPI application and analytics endpoints
├── models.py                  # Pydantic models for requests/responses
├── analytics_service.py       # Core analytics algorithms and data processing
├── openai_service.py          # Placeholder for future AI integrations
├── openapi.json              # OpenAPI specification
├── requirements.txt          # Python dependencies
├── pytest.ini               # Pytest configuration
├── README.md                # This file
└── tests/                   # Test directory
    └── (analytics tests)    # Unit and integration tests
```

## Analytics Capabilities

### Correlation Analysis
- **Pearson Correlation**: Linear relationships between metrics
- **Spearman Correlation**: Monotonic relationships (rank-based)
- **Statistical Significance**: P-value analysis for correlation strength
- **Multiple Metrics**: Analyze relationships between various operational factors

### Forecasting Models
- **Linear Trend Analysis**: Time-series prediction based on historical patterns  
- **Confidence Intervals**: Upper and lower bounds for predictions
- **Trend Detection**: Identify upward, downward, or stable revenue patterns
- **Accuracy Metrics**: Model performance indicators

## Integration with .NET API

This service integrates with the main .NET API to:
- Fetch real restaurant performance data
- Provide advanced analytics capabilities
- Support data-driven decision making

Container orchestration is handled at the .NET Aspire level.