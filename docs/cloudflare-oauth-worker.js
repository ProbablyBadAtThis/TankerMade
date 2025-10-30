/**
 * Cloudflare Worker for GitHub OAuth Token Exchange
 * Deploy this to Cloudflare Workers for secure OAuth token handling
 */

export default {
    async fetch(request, env, ctx) {
        // Handle CORS preflight requests
        if (request.method === 'OPTIONS') {
            return new Response(null, {
                status: 200,
                headers: {
                    'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*',
                    'Access-Control-Allow-Methods': 'POST, OPTIONS',
                    'Access-Control-Allow-Headers': 'Content-Type',
                    'Access-Control-Max-Age': '86400',
                }
            });
        }

        // Handle OAuth token exchange
        if (request.method === 'POST' && request.url.includes('/api/oauth/token')) {
            try {
                const { code } = await request.json();

                if (!code) {
                    return new Response(JSON.stringify({ error: 'Missing authorization code' }), {
                        status: 400,
                        headers: {
                            'Content-Type': 'application/json',
                            'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*'
                        }
                    });
                }

                // Exchange code for access token with GitHub
                const tokenResponse = await fetch('https://github.com/login/oauth/access_token', {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'User-Agent': 'TankerMade-OAuth-Worker/1.0'
                    },
                    body: JSON.stringify({
                        client_id: env.GITHUB_CLIENT_ID,
                        client_secret: env.GITHUB_CLIENT_SECRET,
                        code: code
                    })
                });

                const tokenData = await tokenResponse.json();

                if (tokenData.error) {
                    return new Response(JSON.stringify({ 
                        error: 'GitHub OAuth error', 
                        details: tokenData.error_description 
                    }), {
                        status: 400,
                        headers: {
                            'Content-Type': 'application/json',
                            'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*'
                        }
                    });
                }

                // Return only the access token (don't log sensitive data)
                return new Response(JSON.stringify({ 
                    access_token: tokenData.access_token 
                }), {
                    status: 200,
                    headers: {
                        'Content-Type': 'application/json',
                        'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*',
                        'Cache-Control': 'no-store'
                    }
                });

            } catch (error) {
                console.error('OAuth token exchange error:', error);

                return new Response(JSON.stringify({ 
                    error: 'Internal server error',
                    message: 'Failed to process OAuth token exchange'
                }), {
                    status: 500,
                    headers: {
                        'Content-Type': 'application/json',
                        'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*'
                    }
                });
            }
        }

        // Handle other requests
        return new Response('Not Found', { 
            status: 404,
            headers: {
                'Access-Control-Allow-Origin': env.ALLOWED_ORIGIN || '*'
            }
        });
    }
};

/**
 * DEPLOYMENT INSTRUCTIONS:
 * 
 * 1. Install Wrangler CLI: npm install -g wrangler
 * 
 * 2. Login to Cloudflare: wrangler login
 * 
 * 3. Create wrangler.toml in your project root:
 * 
 * name = "tankermade-oauth-worker"
 * main = "worker.js"
 * compatibility_date = "2024-10-30"
 * 
 * [vars]
 * ALLOWED_ORIGIN = "https://tankermade.pages.dev"
 * 
 * 4. Set secrets (replace with your actual values):
 * wrangler secret put GITHUB_CLIENT_ID
 * wrangler secret put GITHUB_CLIENT_SECRET
 * 
 * 5. Deploy: wrangler deploy
 * 
 * 6. Add custom domain (optional):
 * - Go to Cloudflare dashboard > Workers > Your Worker > Triggers
 * - Add Custom Domain: api.tankermade.pages.dev
 * 
 * 7. Update your GitHub OAuth app settings:
 * - Authorization callback URL: https://tankermade.pages.dev/auth/callback.html
 * - Homepage URL: https://tankermade.pages.dev
 * 
 * 8. Update docs/js/auth.js:
 * - Set clientId to your GitHub OAuth app client ID
 * - Ensure callback endpoint matches your worker URL
 */