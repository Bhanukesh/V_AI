'use client'

import Link from "next/link";
import { useGetRestaurantsQuery } from "@/store/api/enhanced/api";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { BarChart3, TrendingUp, Users, DollarSign } from "lucide-react";

export default function HomePage() {
    const { data: restaurants, isLoading, isError } = useGetRestaurantsQuery();

    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-lg">Loading restaurants...</div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-lg text-red-600">Error loading restaurant data.</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
            <div className="container mx-auto px-4 py-8">
                {/* Header */}
                <div className="text-center mb-12">
                    <h1 className="text-4xl font-bold text-gray-900 mb-4">
                        Vida AI Restaurant Performance Management
                    </h1>
                    <p className="text-xl text-gray-600 max-w-2xl mx-auto">
                        Advanced analytics and insights for restaurant performance optimization
                    </p>
                </div>

                {/* Key Features */}
                <div className="grid md:grid-cols-3 gap-6 mb-12">
                    <Card className="text-center">
                        <CardHeader>
                            <TrendingUp className="h-12 w-12 text-blue-600 mx-auto mb-2" />
                            <CardTitle>Performance Analytics</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <p className="text-gray-600">
                                Track revenue, customer satisfaction, and operational metrics in real-time
                            </p>
                        </CardContent>
                    </Card>

                    <Card className="text-center">
                        <CardHeader>
                            <BarChart3 className="h-12 w-12 text-green-600 mx-auto mb-2" />
                            <CardTitle>Correlation Analysis</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <p className="text-gray-600">
                                Discover relationships between operational metrics and business outcomes
                            </p>
                        </CardContent>
                    </Card>

                    <Card className="text-center">
                        <CardHeader>
                            <Users className="h-12 w-12 text-purple-600 mx-auto mb-2" />
                            <CardTitle>Multi-Location Support</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <p className="text-gray-600">
                                Manage and compare performance across multiple restaurant locations
                            </p>
                        </CardContent>
                    </Card>
                </div>

                {/* Restaurant List */}
                {restaurants && restaurants.length > 0 && (
                    <Card className="mb-8">
                        <CardHeader>
                            <CardTitle className="flex items-center gap-2">
                                <DollarSign className="h-5 w-5" />
                                Your Restaurants ({restaurants.length})
                            </CardTitle>
                            <CardDescription>
                                Click on any restaurant to view detailed analytics and performance metrics
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
                                {restaurants.map((restaurant) => (
                                    <Card key={restaurant.id} className="hover:shadow-lg transition-shadow cursor-pointer">
                                        <CardHeader className="pb-2">
                                            <CardTitle className="text-lg">{restaurant.name}</CardTitle>
                                        </CardHeader>
                                        <CardContent>
                                            <p className="text-sm text-gray-600 mb-3">
                                                {restaurant.address}
                                            </p>
                                            <div className="flex gap-2">
                                                <Link href={`/analytics?restaurantId=${restaurant.id}`} className="flex-1">
                                                    <Button className="w-full" size="sm">
                                                        View Analytics
                                                    </Button>
                                                </Link>
                                            </div>
                                        </CardContent>
                                    </Card>
                                ))}
                            </div>
                        </CardContent>
                    </Card>
                )}

                {/* Call to Action */}
                <div className="text-center">
                    <div className="flex justify-center gap-4">
                        <Link href="/analytics">
                            <Button size="lg" className="flex items-center gap-2">
                                <BarChart3 className="h-5 w-5" />
                                View Analytics Dashboard
                            </Button>
                        </Link>
                        {restaurants && restaurants.length > 0 && (
                            <Link href={`/analytics?restaurantId=${restaurants[0].id}`}>
                                <Button variant="outline" size="lg" className="flex items-center gap-2">
                                    <TrendingUp className="h-5 w-5" />
                                    Sample Restaurant Data
                                </Button>
                            </Link>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}