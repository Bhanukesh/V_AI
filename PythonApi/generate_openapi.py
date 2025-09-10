#!/usr/bin/env python3
"""
Generate OpenAPI spec for the FastAPI application
"""
import json
from main import app

def generate_openapi():
    """Generate and save OpenAPI specification"""
    try:
        # Generate OpenAPI schema
        openapi_schema = app.openapi()
        
        # Pretty print the JSON
        with open('openapi.json', 'w') as f:
            json.dump(openapi_schema, f, indent=2)
        
        print("✅ OpenAPI spec generated successfully!")
        print(f"📋 Title: {openapi_schema['info']['title']}")
        print(f"📋 Version: {openapi_schema['info']['version']}")
        print(f"📋 Endpoints: {len(openapi_schema['paths'])} endpoints")
        
        # List endpoints
        print("\n📡 Available endpoints:")
        for path, methods in openapi_schema['paths'].items():
            for method, details in methods.items():
                if method.upper() != 'OPTIONS':
                    print(f"  {method.upper()} {path}")
                    
        return True
    except Exception as e:
        print(f"❌ Error generating OpenAPI spec: {e}")
        return False

if __name__ == "__main__":
    generate_openapi()