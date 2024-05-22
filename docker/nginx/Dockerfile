FROM nginx:latest

# Install dependencies
RUN apt-get update && \
    apt-get install -y curl gnupg2 ca-certificates lsb-release

# Install OpenTelemetry Nginx module
RUN mkdir -p /etc/nginx/modules
RUN curl -L https://github.com/open-telemetry/opentelemetry-cpp-contrib/releases/latest/download/libopentelemetry_nginx_module.so -o /etc/nginx/modules/ngx_http_opentelemetry_module.so

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf