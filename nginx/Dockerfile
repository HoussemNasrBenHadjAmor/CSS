# nginx/Dockerfile
FROM nginx:stable-alpine

# Remove default config
RUN rm /etc/nginx/conf.d/default.conf

# Copy main nginx.conf to proper place
COPY nginx.conf /etc/nginx/nginx.conf

# Copy initial frontend.conf (with upstream & server blocks)
COPY conf.d/frontend.conf /etc/nginx/conf.d/frontend.conf