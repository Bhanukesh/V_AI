from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import RedirectResponse
from datetime import datetime
from models import (
    CorrelationRequest, CorrelationResponse, 
    ForecastRequest, ForecastResponse
)
from analytics_service import AnalyticsService

app = FastAPI(
    title="Vida AI Analytics Engine",
    description="Advanced analytics service for restaurant performance management with statistical correlation analysis and revenue forecasting",
    version="1.0.0",
    openapi_url="/openapi.json",
    docs_url="/docs",
    redoc_url="/redoc"
)

# Initialize analytics service
analytics_service = AnalyticsService()

# Configure CORS to allow all origins
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allow all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allow all HTTP methods
    allow_headers=["*"],  # Allow all headers
)

# Send interactive user to swagger page by default
@app.get("/")
async def redirect_to_swagger():
    return RedirectResponse(url="/docs")


# Health check endpoint
@app.get("/health")
async def health_check():
    """Health check endpoint for service monitoring"""
    return {"status": "healthy", "timestamp": datetime.utcnow()}


# Statistical Analysis Endpoints
@app.post("/analytics/correlation", response_model=CorrelationResponse)
async def calculate_correlation(request: CorrelationRequest):
    """
    Calculate correlations between operational metrics and revenue.
    Fetches real data from .NET API and performs correlation analysis.
    """
    try:
        # Fetch real data for the restaurant and calculate correlations with revenue
        correlations = await analytics_service.calculate_revenue_correlations(
            request.restaurant_id,
            request.metrics or ["prep_time", "table_turnover", "order_accuracy", "customer_satisfaction", "wait_time"],
            request.correlation_type
        )
        
        return CorrelationResponse(
            restaurant_id=request.restaurant_id,
            correlations=correlations,
            total_data_points=len(correlations),
            analysis_timestamp=datetime.utcnow()
        )
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail=f"Correlation analysis failed: {str(e)}"
        )


@app.post("/analytics/forecast", response_model=ForecastResponse)
async def forecast_revenue(request: ForecastRequest):
    """
    Generate revenue forecasts using linear trend analysis.
    Provides predictions with confidence intervals.
    """
    try:
        forecast_points, accuracy, trend = analytics_service.forecast_revenue(
            request.historical_data,
            request.forecast_days
        )
        
        if not forecast_points:
            raise HTTPException(
                status_code=400,
                detail="Insufficient data for forecasting. Need at least 7 days of historical data."
            )
        
        return ForecastResponse(
            restaurant_id=request.restaurant_id,
            forecast_points=forecast_points,
            model_accuracy=accuracy,
            trend_direction=trend,
            analysis_timestamp=datetime.utcnow()
        )
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Forecasting failed: {str(e)}"
        )