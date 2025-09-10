from pydantic import BaseModel
from typing import Optional, Literal, List, Dict, Any
from enum import Enum
from datetime import datetime


# Statistical Analysis Models

class CorrelationType(str, Enum):
    PEARSON = "pearson"
    SPEARMAN = "spearman"


class DataPoint(BaseModel):
    timestamp: datetime
    value: float
    metric_name: str


class CorrelationRequest(BaseModel):
    restaurant_id: int
    metrics: Optional[List[str]] = None  # List of metric names to correlate with revenue
    correlation_type: str = "pearson"


class CorrelationPair(BaseModel):
    metric1: str
    metric2: str
    correlation_coefficient: float
    p_value: float
    strength: str
    significant: bool


class CorrelationResponse(BaseModel):
    restaurant_id: int
    correlations: List[CorrelationPair]
    total_data_points: int
    analysis_timestamp: datetime


class ForecastRequest(BaseModel):
    historical_data: List[Dict[str, Any]]  # Revenue data with date and amount
    forecast_days: int = 30
    restaurant_id: int


class ForecastPoint(BaseModel):
    date: str
    predicted_value: float
    confidence_interval_lower: float
    confidence_interval_upper: float


class ForecastResponse(BaseModel):
    model_config = {"protected_namespaces": ()}  # Fix Pydantic warning
    
    restaurant_id: int
    forecast_points: List[ForecastPoint]
    model_accuracy: float
    trend_direction: str  # "up", "down", "stable"
    analysis_timestamp: datetime