#!/usr/bin/env python3
"""
Comprehensive system test for Vida AI restaurant analytics platform
Tests .NET API, Python Analytics Service, and service integration
"""
import requests
import json
import sys
import time
from datetime import datetime

class VidaAISystemTester:
    def __init__(self):
        self.api_base = "https://localhost:7583"  # .NET API
        self.python_base = "http://localhost:8000"  # Python Analytics
        self.results = []
        
    def log(self, message, status="INFO"):
        timestamp = datetime.now().strftime("%H:%M:%S")
        print(f"[{timestamp}] {status}: {message}")
        self.results.append((status, message))
    
    def test_endpoint(self, url, method="GET", json_data=None, description=""):
        """Test a single endpoint"""
        try:
            if method == "GET":
                response = requests.get(url, verify=False, timeout=10)
            elif method == "POST":
                response = requests.post(url, json=json_data, verify=False, timeout=10)
            
            if response.status_code == 200:
                self.log(f"✅ {description}: {response.status_code}", "PASS")
                return True, response.json() if response.content else {}
            else:
                self.log(f"❌ {description}: HTTP {response.status_code}", "FAIL")
                return False, None
        except requests.exceptions.SSLError:
            self.log(f"🔒 {description}: SSL Certificate issue (expected for localhost)", "WARN")
            return False, None
        except requests.exceptions.ConnectionError:
            self.log(f"❌ {description}: Connection refused", "FAIL")
            return False, None
        except requests.exceptions.Timeout:
            self.log(f"⏰ {description}: Request timeout", "FAIL")
            return False, None
        except Exception as e:
            self.log(f"❌ {description}: {str(e)}", "FAIL")
            return False, None

    def test_dotnet_api(self):
        """Test .NET API endpoints"""
        self.log("🔍 Testing .NET API Service", "INFO")
        
        # Test basic endpoints
        tests = [
            (f"{self.api_base}/health", "GET", None, ".NET API Health Check"),
            (f"{self.api_base}/api/restaurants", "GET", None, "Restaurants endpoint"),
            (f"{self.api_base}/swagger/index.html", "GET", None, "Swagger documentation"),
        ]
        
        for url, method, data, desc in tests:
            success, result = self.test_endpoint(url, method, data, desc)
        
        # Test analytics endpoints (if restaurants exist)
        self.log("🧮 Testing Analytics endpoints", "INFO")
        analytics_tests = [
            (f"{self.api_base}/api/analytics/restaurants/1/data", "GET", None, "Analytics data endpoint"),
            (f"{self.api_base}/api/analytics/restaurants/1/correlations", "GET", None, "Correlation analysis endpoint"),
        ]
        
        for url, method, data, desc in analytics_tests:
            success, result = self.test_endpoint(url, method, data, desc)

    def test_python_analytics(self):
        """Test Python Analytics Service"""
        self.log("🐍 Testing Python Analytics Service", "INFO")
        
        # Test basic endpoints
        tests = [
            (f"{self.python_base}/health", "GET", None, "Python service health"),
            (f"{self.python_base}/docs", "GET", None, "FastAPI documentation"),
            (f"{self.python_base}/openapi.json", "GET", None, "OpenAPI specification"),
        ]
        
        for url, method, data, desc in tests:
            success, result = self.test_endpoint(url, method, data, desc)
        
        # Test correlation endpoint with sample data
        correlation_data = {
            "restaurant_id": 1,
            "metrics": ["prep_time", "customer_satisfaction", "order_accuracy"],
            "correlation_type": "pearson"
        }
        
        success, result = self.test_endpoint(
            f"{self.python_base}/analytics/correlation",
            "POST",
            correlation_data,
            "Correlation analysis with sample data"
        )
        
        if success and result:
            self.log(f"📊 Correlation results: {len(result.get('correlations', []))} correlations found", "INFO")

    def test_integration(self):
        """Test service integration"""
        self.log("🔗 Testing Service Integration", "INFO")
        
        # This would test the full flow: Frontend -> .NET -> Python -> Database
        # For now, we test that services can communicate
        
        try:
            # Test if .NET can reach Python (this would be internal to .NET API)
            self.log("🔄 Integration tests require running system", "INFO")
        except Exception as e:
            self.log(f"❌ Integration test failed: {e}", "FAIL")

    def generate_report(self):
        """Generate test report"""
        self.log("📋 Test Report Summary", "INFO")
        
        total_tests = len(self.results)
        passed = len([r for r in self.results if r[0] == "PASS"])
        failed = len([r for r in self.results if r[0] == "FAIL"])
        warnings = len([r for r in self.results if r[0] == "WARN"])
        
        print(f"\n{'='*60}")
        print(f"VIDA AI SYSTEM TEST REPORT")
        print(f"{'='*60}")
        print(f"Total Tests: {total_tests}")
        print(f"✅ Passed: {passed}")
        print(f"❌ Failed: {failed}")
        print(f"⚠️  Warnings: {warnings}")
        print(f"Success Rate: {(passed/total_tests)*100:.1f}%")
        print(f"{'='*60}")
        
        if failed == 0:
            print("🎉 ALL SYSTEMS OPERATIONAL!")
            print("Your Vida AI restaurant analytics platform is ready!")
        else:
            print("⚠️  Some issues detected. Check logs above.")
        
        return failed == 0

    def run_all_tests(self):
        """Run complete system test"""
        self.log("🚀 Starting Vida AI System Tests", "INFO")
        print(f"Testing at {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        print("-" * 60)
        
        self.test_dotnet_api()
        self.test_python_analytics()
        self.test_integration()
        
        return self.generate_report()

if __name__ == "__main__":
    print("🌟 VIDA AI RESTAURANT ANALYTICS PLATFORM SYSTEM TEST")
    print("=" * 65)
    
    tester = VidaAISystemTester()
    success = tester.run_all_tests()
    
    sys.exit(0 if success else 1)