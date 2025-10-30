export async function onRequestPost(context) {
    const { request, env } = context;
    
    try {
        const { code } = await request.json();
        
        if (!code) {
            return new Response(JSON.stringify({ error: 'Missing authorization code' }), {
                status: 400,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        // Exchange code for access token
        const tokenResponse = await fetch('https://github.com/login/oauth/access_token', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'User-Agent': 'TankerMade-OAuth/1.0'
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
                headers: { 'Content-Type': 'application/json' }
            });
        }

        return new Response(JSON.stringify({ 
            access_token: tokenData.access_token 
        }), {
            status: 200,
            headers: { 
                'Content-Type': 'application/json',
                'Cache-Control': 'no-store'
            }
        });

    } catch (error) {
        return new Response(JSON.stringify({ 
            error: 'Internal server error',
            message: error.message
        }), {
            status: 500,
            headers: { 'Content-Type': 'application/json' }
        });
    }
}
