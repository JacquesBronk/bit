FROM nginx:latest

# Install dependencies
RUN apt-get update && \
    apt-get install -y curl gnupg2 ca-certificates lsb-release

COPY nginx.conf /etc/nginx/nginx.conf
