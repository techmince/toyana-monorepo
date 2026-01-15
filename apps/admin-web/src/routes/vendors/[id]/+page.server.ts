export const load = async ({ params }) => {
    // TODO: Call Gateway /vendors/{id}
    return {
        vendor: {
            id: params.id,
            name: "Creative Lens Studio",
            location: "New York, NY",
            rating: 4.9,
            reviews: 124,
            description: "We are a team of passionate photographers capturing the essence of your special moments. From intimate weddings to grand corporate events, we bring a creative touch to every shot.",
            services: [
                { id: "1", name: "Full Wedding Package", price: 3500, duration: 480, description: "8 hours of coverage, 2 photographers, online gallery." },
                { id: "2", name: "Engagement Session", price: 500, duration: 90, description: "1.5 hours session at a location of your choice." }
            ]
        }
    };
};
