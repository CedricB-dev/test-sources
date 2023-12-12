import React, {useState} from 'react';

export const Counter : React.FC = () => {
    const [currentCount, setCurrentCount ] = useState<number>(0);

    return (
        <div>
            <h1>Counter</h1>

            <p>This is a simple example of a React component.</p>

            <p aria-live="polite">Current count: <strong>{currentCount}</strong></p>

            <button className="btn btn-primary" onClick={() => setCurrentCount(p => p +1)}>Increment</button>
        </div>
    );
};
